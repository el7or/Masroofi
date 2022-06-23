using Puzzle.Masroofi.Core;
using Puzzle.Masroofi.Core.Models;

namespace Puzzle.Masroofi.Infrastructure.Repositories
{
    public class ATMCardRepository : RepositoryBase<ATMCard>, IATMCardRepository
    {
        public ATMCardRepository(MasroofiDbContext dbContext) : base(dbContext)
        {
        }
    }

    public interface IATMCardRepository : IRepository<ATMCard>
    {
    }
}
