using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PlanPK.Repositories;
using PlanPK.Entities;

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
            Issues = _issueRepository.GetIssues();
        }
    }
}
