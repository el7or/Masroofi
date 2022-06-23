using Puzzle.Masroofi.Core;
using Puzzle.Masroofi.Core.Models;

namespace Puzzle.Masroofi.Infrastructure.Repositories
{
    public class CommissionRepository : RepositoryBase<CommissionSetting>, ICommissionRepository
    {
        public CommissionRepository(MasroofiDbContext dbContext) : base(dbContext)
        {
        }
    }

    public interface ICommissionRepository : IRepository<CommissionSetting>
    {
    }
}
