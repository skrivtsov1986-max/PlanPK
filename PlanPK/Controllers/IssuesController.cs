using Microsoft.AspNetCore.Mvc;
using PlanPK.Repositories;
using PlanPK.Entities;
using PlanPK.Services;

namespace PlanPK.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class IssuesController : ControllerBase
    {
        private readonly IssueRepository _repository;
        private readonly IssueSyncService _syncService;

        public IssuesController(IssueRepository repository, IssueSyncService syncService)
        {
            _repository = repository;
            _syncService = syncService;
        }

        [HttpGet]
        public async Task<IActionResult> GetTree(CancellationToken cancellationToken)
        {
            var nodes = await _repository.GetIssuesAsync(cancellationToken);
            var tree = BuildTree(nodes, null); // Передаем корневой узел
            return Ok(tree);
        }

        [HttpPost("sync")]
        public async Task<IActionResult> SyncFromYouTrack(CancellationToken cancellationToken)
        {
            var syncResult = await _syncService.RefreshIssuesAsync(cancellationToken);
            return Ok(syncResult);
        }

        private List<Issues> BuildTree(List<Issues> nodes, string? parentId)
        {
            var tree = new List<Issues>();
            foreach (var node in nodes.Where(n => n.ParentId == parentId))
            {
                node.ChildrenIssue = BuildTree(nodes, node.Id);
                tree.Add(node);
            }
            return tree;
        }
    }
}
