using Microsoft.EntityFrameworkCore;
using PlanPK.Entities;

namespace PlanPK.Repositories
{
    public class IssueRepository
    {
        private readonly ApplicationDbContext _appDbContext;

        public IssueRepository(ApplicationDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public Task<List<Issues>> GetIssuesAsync(CancellationToken cancellationToken = default)
        {
            return _appDbContext.Issues.AsNoTracking().ToListAsync(cancellationToken);
        }

        public async Task ReplaceIssuesAsync(IEnumerable<Issues> issues, CancellationToken cancellationToken = default)
        {
            await _appDbContext.Issues.ExecuteDeleteAsync(cancellationToken);
            await _appDbContext.AddRangeAsync(issues, cancellationToken);
            await _appDbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
