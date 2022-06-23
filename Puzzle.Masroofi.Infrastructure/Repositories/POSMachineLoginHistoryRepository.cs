using Puzzle.Masroofi.Core;
using Puzzle.Masroofi.Core.Models;

namespace Puzzle.Masroofi.Infrastructure.Repositories
{
    public class POSMachineLoginHistoryRepository : RepositoryBase<POSMachineLoginHistory>, IPOSMachineLoginHistoryRepository
    {
        public POSMachineLoginHistoryRepository(MasroofiDbContext dbContext) : base(dbContext)
        {
        }
    }
    public interface IPOSMachineLoginHistoryRepository : IRepository<POSMachineLoginHistory>
    {
    }
}
