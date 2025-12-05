using Microsoft.AspNetCore.Mvc;
using PlanPK.Repositories;
using PlanPK.Entities;

namespace PlanPK.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class IssuesController : ControllerBase
    {
        private readonly IssueRepository _repository;

        public IssuesController(IssueRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> GetTree()
        {
            var nodes = _repository.GetIssues();
            var tree = BuildTree(nodes, null); // Передаем корневой узел
            return Ok(tree);
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
