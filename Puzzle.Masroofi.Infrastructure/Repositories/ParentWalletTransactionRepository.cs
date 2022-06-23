using Puzzle.Masroofi.Core;
using Puzzle.Masroofi.Core.Models;

namespace Puzzle.Masroofi.Infrastructure.Repositories
{
    public class ParentWalletTransactionRepository : RepositoryBase<ParentWalletTransaction>, IParentWalletTransactionRepository
    {
        public ParentWalletTransactionRepository(MasroofiDbContext dbContext) : base(dbContext)
        {
        }
    }

    public interface IParentWalletTransactionRepository : IRepository<ParentWalletTransaction>
    {
    }
}
