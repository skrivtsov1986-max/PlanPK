using Microsoft.Data.SqlClient;
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
        public List<Issues> GetIssues()
        {
            var result = _appDbContext.Database.SqlQueryRaw<Issues>(

            $"Select * from dbo.Issues;").ToList();
            return result;
        }
    }
}
