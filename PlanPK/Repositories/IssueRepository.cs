using Microsoft.EntityFrameworkCore;
using PlanPK.Entities;

namespace PlanPK.Repositories
{
    public class IssueRepository
    {
        private readonly ApplicationDbContext _appDbContext;
        public IssueRepository(ApplicationDbContext appDbContext_)
        {
            _appDbContext = appDbContext_;
        }
        public async Task<List<Issues>> GetIssuesAsync(CancellationToken cancellationToken = default)
        {
            return await _appDbContext.Issues
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
    }
}
