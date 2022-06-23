using Puzzle.Masroofi.Core;
using Puzzle.Masroofi.Core.Models;

namespace Puzzle.Masroofi.Infrastructure.Repositories
{
    public class ATMCardTransactionRepository : RepositoryBase<ATMCardTransaction>, IATMCardTransactionRepository
    {
        public ATMCardTransactionRepository(MasroofiDbContext dbContext) : base(dbContext)
        {
        }
    }

    public interface IATMCardTransactionRepository : IRepository<ATMCardTransaction>
    {
    }
}
