using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PlanPK.Repositories;
using PlanPK.Entities;
using System.Linq;

namespace PlanPK.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IssueRepository _issueRepository;

        public List<Issues> Issues { get; private set; } = new();

        public IndexModel(ILogger<IndexModel> logger, IssueRepository issueRepository)
        {
            _logger = logger;
            _issueRepository = issueRepository;
        }

        public void OnGet()
        {
            var allIssues = _issueRepository.GetIssues();
            Issues = BuildIssueTree(allIssues);
        }

        private static List<Issues> BuildIssueTree(List<Issues> issues)
        {
            if (issues.Count == 0)
            {
                return new List<Issues>();
            }

            var issuesById = issues.ToDictionary(issue => issue.Id);
            var roots = new List<Issues>();

            foreach (var issue in issues)
            {
                if (!string.IsNullOrEmpty(issue.ParentId) && issuesById.TryGetValue(issue.ParentId, out var parentIssue))
                {
                    parentIssue.ChildrenIssue.Add(issue);
                }
                else
                {
                    roots.Add(issue);
                }
            }

            var orderedRoots = roots.OrderBy(issue => issue.Number).ToList();

            foreach (var root in orderedRoots)
            {
                SortChildren(root);
            }

            return orderedRoots;
        }

        private static void SortChildren(Issues issue)
        {
            if (issue.ChildrenIssue.Count == 0)
            {
                return;
            }

            issue.ChildrenIssue = issue.ChildrenIssue
                .OrderBy(child => child.Number)
                .ToList();

            foreach (var childIssue in issue.ChildrenIssue)
            {
                SortChildren(childIssue);
            }
        }
    }
}
