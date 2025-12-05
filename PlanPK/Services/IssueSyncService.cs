using System.Globalization;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PlanPK.Services.YouTrack;

namespace PlanPK.Services;

public class IssueSyncService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly YouTrackClient _youTrackClient;
    private readonly YouTrackOptions _options;
    private readonly ILogger<IssueSyncService> _logger;

    public IssueSyncService(
        ApplicationDbContext dbContext,
        YouTrackClient youTrackClient,
        IOptions<YouTrackOptions> options,
        ILogger<IssueSyncService> logger)
    {
        _dbContext = dbContext;
        _youTrackClient = youTrackClient;
        _logger = logger;
        _options = options.Value;
    }

    public async Task<IssueSyncResult> RefreshIssuesAsync(CancellationToken cancellationToken = default)
    {
        var remoteIssues = await _youTrackClient.GetIssuesAsync(cancellationToken);
        var mappedIssues = remoteIssues.Select(MapIssue).ToList();

        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        _dbContext.Issues.RemoveRange(_dbContext.Issues);
        await _dbContext.SaveChangesAsync(cancellationToken);

        await _dbContext.Issues.AddRangeAsync(mappedIssues, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        await transaction.CommitAsync(cancellationToken);

        _logger.LogInformation("Replaced local Issues table with {Count} records from YouTrack", mappedIssues.Count);
        return new IssueSyncResult(mappedIssues.Count);
    }

    private Issues MapIssue(YouTrackIssueDto issueDto)
    {
        return new Issues
        {
            Id = issueDto.Id,
            Number = issueDto.Number,
            Name = issueDto.Name,
            ParentId = issueDto.Parent?.Id,
            PlanHours = ExtractHours(issueDto, _options.PlanHoursFieldName),
            FaktHours = ExtractHours(issueDto, _options.FaktHoursFieldName),
            TermDate = ExtractDate(issueDto, _options.TermDateFieldName),
            ChildrenIssue = new List<Issues>()
        };
    }

    private double ExtractHours(YouTrackIssueDto issueDto, string fieldName)
    {
        var field = issueDto.CustomFields.FirstOrDefault(f => string.Equals(f.Name, fieldName, StringComparison.OrdinalIgnoreCase));
        if (field?.Value is null)
        {
            return 0;
        }

        var value = field.Value.Value;

        try
        {
            if (value.ValueKind == JsonValueKind.Number && value.TryGetDouble(out var numeric))
            {
                return numeric;
            }

            if (value.ValueKind == JsonValueKind.Object)
            {
                if (value.TryGetProperty("minutes", out var minutesElement) && minutesElement.TryGetDouble(out var minutes))
                {
                    return minutes / 60.0;
                }

                if (value.TryGetProperty("value", out var innerValue))
                {
                    if (innerValue.ValueKind == JsonValueKind.Number && innerValue.TryGetDouble(out var number))
                    {
                        return number;
                    }
                }

                if (value.TryGetProperty("presentation", out var presentation) &&
                    double.TryParse(presentation.GetString(), NumberStyles.Any, CultureInfo.InvariantCulture, out var parsed))
                {
                    return parsed;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to parse hours from field {Field} in issue {Issue}", fieldName, issueDto.Id);
        }

        return 0;
    }

    private DateTime? ExtractDate(YouTrackIssueDto issueDto, string fieldName)
    {
        var field = issueDto.CustomFields.FirstOrDefault(f => string.Equals(f.Name, fieldName, StringComparison.OrdinalIgnoreCase));
        if (field?.Value is null)
        {
            return null;
        }

        var value = field.Value.Value;

        try
        {
            if (value.ValueKind == JsonValueKind.Number && value.TryGetInt64(out var timestamp))
            {
                return DateTimeOffset.FromUnixTimeMilliseconds(timestamp).DateTime;
            }

            if (value.ValueKind == JsonValueKind.Object)
            {
                if (value.TryGetProperty("date", out var dateElement) && dateElement.TryGetInt64(out var dateTimestamp))
                {
                    return DateTimeOffset.FromUnixTimeMilliseconds(dateTimestamp).DateTime;
                }

                if (value.TryGetProperty("presentation", out var presentation))
                {
                    var stringValue = presentation.GetString();
                    if (DateTime.TryParse(stringValue, out var parsedDate))
                    {
                        return parsedDate;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to parse date from field {Field} in issue {Issue}", fieldName, issueDto.Id);
        }

        return null;
    }
}

public record IssueSyncResult(int ImportedCount);
