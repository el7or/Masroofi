using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Puzzle.Masroofi.Core.Extensions;
using Puzzle.Masroofi.Core.Helpers;
using Puzzle.Masroofi.Core.Enums;
using Puzzle.Masroofi.Core.Models;
using Puzzle.Masroofi.Core.ViewModels;
using Puzzle.Masroofi.Core.ViewModels.Auth;
using Puzzle.Masroofi.Core.ViewModels.Governorates;
using Puzzle.Masroofi.Infrastructure;
using Puzzle.Masroofi.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Puzzle.Masroofi.ServiceInterface
{
    public interface IGovernorateService
    {
        Task<PagedOutput<GovernorateOutputViewModel>> GetAllAsync(GovernorateFilterViewModel model);
        Task<GovernorateOutputViewModel> GetAsync(int governorateId);
    }

    public class GovernorateService : BaseService, IGovernorateService
    {
        private readonly IGovernorateRepository _governorateRepository;
        private readonly UserIdentity _userIdentity;

        public GovernorateService(IGovernorateRepository governorateRepository,
            UserIdentity userIdentity,
            IUnitOfWork unitOfWork,
            IMapper mapper) : base(unitOfWork, mapper)
        {
            _governorateRepository = governorateRepository;
            _userIdentity = userIdentity;
        }

        public async Task<PagedOutput<GovernorateOutputViewModel>> GetAllAsync(GovernorateFilterViewModel model)
        {
            var result = new PagedOutput<GovernorateOutputViewModel>();

            var query = _governorateRepository.Table
                .AsQueryable();

            // filtering
            query = FilterGovernorates(query, model);

            // sorting
            var columnsMap = new Dictionary<string, Expression<Func<Governorate, object>>>()
            {
                ["governorateName"] = v => _userIdentity.Language == Language.en ? v.GovernorateNameEn : v.GovernorateNameAr
            };
            query = query.ApplySorting(model, columnsMap);

            result.TotalCount = await query.CountAsync();

            // paging
            query = query.ApplyPaging(model);

            result.Result = mapper.Map<List<GovernorateOutputViewModel>>(query);

            return result;
        }

        public async Task<GovernorateOutputViewModel> GetAsync(int governorateId)
        {
            var entityToGet = await _governorateRepository.Table
                .FirstOrDefaultAsync(c => c.GovernorateId == governorateId);
            if (entityToGet == null)
            {
                throw new BusinessException("Not found");
            }

            return mapper.Map<GovernorateOutputViewModel>(entityToGet);
        }

        private IQueryable<Governorate> FilterGovernorates(IQueryable<Governorate> query, GovernorateFilterViewModel queryObject)
        {
            if (queryObject.GovernorateId.HasValue)
            {
                query = query.Where(c => c.GovernorateId == queryObject.GovernorateId);
            }
            if (!string.IsNullOrEmpty(queryObject.SearchText))
            {
                query = query.Where(c => _userIdentity.Language == Language.en ? c.GovernorateNameEn.Contains(queryObject.SearchText) :
                _userIdentity.Language == Language.ar ? c.GovernorateNameAr.Contains(queryObject.SearchText) :
                c.GovernorateNameAr.Contains(queryObject.SearchText) || c.GovernorateNameEn.Contains(queryObject.SearchText));
            }

            return query;
        }
    }
}
