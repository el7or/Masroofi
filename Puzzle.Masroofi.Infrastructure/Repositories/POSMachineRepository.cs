using Puzzle.Masroofi.Core;
using Puzzle.Masroofi.Core.Models;

namespace Puzzle.Masroofi.Infrastructure.Repositories
{
    public class POSMachineRepository : RepositoryBase<POSMachine>, IPOSMachineRepository
    {
        public POSMachineRepository(MasroofiDbContext dbContext) : base(dbContext)
        {
        }
    }

    public interface IPOSMachineRepository : IRepository<POSMachine>
    {
    }
}
