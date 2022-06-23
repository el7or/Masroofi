using Puzzle.Masroofi.Core;
using Puzzle.Masroofi.Core.Models;

namespace Puzzle.Masroofi.Infrastructure.Repositories
{
    public class ParentRepository : RepositoryBase<Parent>, IParentRepository
    {
        public ParentRepository(MasroofiDbContext dbContext) : base(dbContext)
        {
        }
    }

    public interface IParentRepository : IRepository<Parent>
    {
    }
}
