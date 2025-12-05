using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.Extensions.Options;

namespace PlanPK.Services.YouTrack;

public class YouTrackClient
{
    private readonly HttpClient _httpClient;
    private readonly YouTrackOptions _options;
    private readonly ILogger<YouTrackClient> _logger;

    private const string Fields = "id,idReadable,summary,parent(id),customFields(name,value(id,fullName,name,presentation,minutes,date,value))";

    public YouTrackClient(HttpClient httpClient, IOptions<YouTrackOptions> options, ILogger<YouTrackClient> logger)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<IReadOnlyList<YouTrackIssueDto>> GetIssuesAsync(CancellationToken cancellationToken = default)
    {
        ValidateOptions();

        var issues = new List<YouTrackIssueDto>();
        var skip = 0;

        while (true)
        {
            var requestUri = BuildIssuesUri(skip);
            using var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _options.Token);

            var response = await _httpClient.SendAsync(request, cancellationToken);
            response.EnsureSuccessStatusCode();

            await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            var page = await JsonSerializer.DeserializeAsync<List<YouTrackIssueDto>>(stream, cancellationToken: cancellationToken) ?? new List<YouTrackIssueDto>();

            if (page.Count == 0)
            {
                break;
            }

            issues.AddRange(page);

            if (page.Count < _options.PageSize)
            {
                break;
            }

            skip += page.Count;
        }

        _logger.LogInformation("Loaded {Count} issues from YouTrack", issues.Count);
        return issues;
    }

    private string BuildIssuesUri(int skip)
    {
        var baseUrl = _options.BaseUrl.TrimEnd('/');
        var query = Uri.EscapeDataString(_options.Query);
        return $"{baseUrl}/api/issues?query={query}&$skip={skip}&$top={_options.PageSize}&fields={Fields}";
    }

    private void ValidateOptions()
    {
        if (string.IsNullOrWhiteSpace(_options.BaseUrl))
        {
            throw new InvalidOperationException("YouTrack base URL is not configured.");
        }

        if (string.IsNullOrWhiteSpace(_options.Token))
        {
            throw new InvalidOperationException("YouTrack access token is not configured.");
        }
    }
}
