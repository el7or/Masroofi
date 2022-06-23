using Puzzle.Masroofi.Infrastructure;
using AutoMapper;

namespace Puzzle.Masroofi.ServiceInterface
{
    public class BaseService
    {
        protected readonly IUnitOfWork unitOfWork;
        protected readonly IMapper mapper;

        public BaseService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }
    }
}
