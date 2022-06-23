using Puzzle.Masroofi.Core;
using Puzzle.Masroofi.Core.Models;

namespace Puzzle.Masroofi.Infrastructure.Repositories
{
    public class ParentPinCodeHistoryRepository : RepositoryBase<ParentPinCodeHistory>, IParentPinCodeHistoryRepository
    {
        public ParentPinCodeHistoryRepository(MasroofiDbContext dbContext) : base(dbContext)
        {
        }
    }

    public interface IParentPinCodeHistoryRepository : IRepository<ParentPinCodeHistory>
    {
    }
}
