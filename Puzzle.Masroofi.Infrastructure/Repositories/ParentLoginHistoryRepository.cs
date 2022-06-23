using Puzzle.Masroofi.Core;
using Puzzle.Masroofi.Core.Models;

namespace Puzzle.Masroofi.Infrastructure.Repositories
{
    public class ParentLoginHistoryRepository : RepositoryBase<ParentLoginHistory>, IParentLoginHistoryRepository
    {
        public ParentLoginHistoryRepository(MasroofiDbContext dbContext) : base(dbContext)
        {
        }
    }
    public interface IParentLoginHistoryRepository : IRepository<ParentLoginHistory>
    {
    }
}
