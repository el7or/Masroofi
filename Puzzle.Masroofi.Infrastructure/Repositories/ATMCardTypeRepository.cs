using Puzzle.Masroofi.Core;
using Puzzle.Masroofi.Core.Models;

namespace Puzzle.Masroofi.Infrastructure.Repositories
{
    public class ATMCardTypeRepository : RepositoryBase<ATMCardType>, IATMCardTypeRepository
    {
        public ATMCardTypeRepository(MasroofiDbContext dbContext) : base(dbContext)
        {
        }
    }

    public interface IATMCardTypeRepository : IRepository<ATMCardType>
    {
    }
}
