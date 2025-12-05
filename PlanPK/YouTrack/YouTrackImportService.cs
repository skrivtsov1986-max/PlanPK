using PlanPK.Repositories;
using PlanPK;

namespace PlanPK.YouTrack
{
    public class YouTrackImportService
    {
        private readonly IssueRepository _issueRepository;
        private readonly YouTrackClient _youTrackClient;
        private readonly ILogger<YouTrackImportService> _logger;

        public YouTrackImportService(IssueRepository issueRepository, YouTrackClient youTrackClient, ILogger<YouTrackImportService> logger)
        {
            _issueRepository = issueRepository;
            _youTrackClient = youTrackClient;
            _logger = logger;
        }

        public async Task RefreshIssuesAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Starting YouTrack import.");
            var remoteIssues = await _youTrackClient.GetIssuesAsync(cancellationToken);
            _logger.LogInformation("Received {Count} issues from YouTrack.", remoteIssues.Count);

            var issuesLookup = remoteIssues
                .Where(issue => !string.IsNullOrWhiteSpace(issue.IdReadable))
                .ToDictionary(
                    issue => issue.IdReadable!,
                    issue => new Issues
                    {
                        Id = issue.IdReadable!,
                        Number = issue.IdReadable!,
                        Name = issue.Summary ?? string.Empty,
                        PlanHours = 0,
                        FaktHours = 0,
                        TermDate = null
                    });

            foreach (var sourceIssue in remoteIssues)
            {
                if (string.IsNullOrWhiteSpace(sourceIssue.IdReadable))
                {
                    continue;
                }

                foreach (var link in sourceIssue.Links)
                {
                    if (!string.Equals(link.LinkType?.Name, "Subtask", StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    if (!string.Equals(link.LinkType?.SourceToTarget, "parent for", StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    foreach (var linkedIssue in link.Issues)
                    {
                        if (linkedIssue.IdReadable is null)
                        {
                            continue;
                        }

                        if (issuesLookup.TryGetValue(linkedIssue.IdReadable, out var childIssue))
                        {
                            childIssue.ParentId = sourceIssue.IdReadable;
                        }
                    }
                }
            }

            var issues = issuesLookup.Values.ToList();
            await _issueRepository.ReplaceIssuesAsync(issues, cancellationToken);
            _logger.LogInformation("YouTrack import completed. {Count} issues saved.", issues.Count);
        }
    }
}
