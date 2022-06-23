using Puzzle.Masroofi.Core;
using Puzzle.Masroofi.Core.Models;

namespace Puzzle.Masroofi.Infrastructure.Repositories
{
    public class POSMachineTransactionRepository : RepositoryBase<POSMachineTransaction>, IPOSMachineTransactionRepository
    {
        public POSMachineTransactionRepository(MasroofiDbContext dbContext) : base(dbContext)
        {
        }
    }

    public interface IPOSMachineTransactionRepository : IRepository<POSMachineTransaction>
    {
    }
}
