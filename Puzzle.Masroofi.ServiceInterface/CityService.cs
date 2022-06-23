using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Puzzle.Masroofi.Core.Extensions;
using Puzzle.Masroofi.Core.Helpers;
using Puzzle.Masroofi.Core.Enums;
using Puzzle.Masroofi.Core.Models;
using Puzzle.Masroofi.Core.ViewModels;
using Puzzle.Masroofi.Core.ViewModels.Auth;
using Puzzle.Masroofi.Core.ViewModels.Cities;
using Puzzle.Masroofi.Infrastructure;
using Puzzle.Masroofi.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Puzzle.Masroofi.ServiceInterface
{
    public interface ICityService
    {
        Task<PagedOutput<CityOutputViewModel>> GetAllAsync(CityFilterViewModel model);
        Task<CityOutputViewModel> GetAsync(int cityId);
    }
    public class CityService : BaseService, ICityService
    {
        private readonly ICityRepository _cityRepository;
        private readonly UserIdentity _userIdentity;

        public CityService(ICityRepository cityRepository,
            UserIdentity userIdentity,
            IUnitOfWork unitOfWork,
            IMapper mapper) : base(unitOfWork, mapper)
        {
            _cityRepository = cityRepository;
            _userIdentity = userIdentity;
        }

        public async Task<PagedOutput<CityOutputViewModel>> GetAllAsync(CityFilterViewModel model)
        {
            var result = new PagedOutput<CityOutputViewModel>();

            var query = _cityRepository.Table
                .AsQueryable();

            // filtering
            query = FilterCities(query, model);

            // sorting
            var columnsMap = new Dictionary<string, Expression<Func<City, object>>>()
            {
                ["cityName"] = v => _userIdentity.Language == Language.en ? v.CityNameEn : v.CityNameAr,
                ["governorateId"] = v => v.GovernorateId
            };
            query = query.ApplySorting(model, columnsMap);

            result.TotalCount = await query.CountAsync();

            // paging
            query = query.ApplyPaging(model);

            result.Result = mapper.Map<List<CityOutputViewModel>>(query);

            return result;
        }

        public async Task<CityOutputViewModel> GetAsync(int cityId)
        {
            var entityToGet = await _cityRepository.Table
                .FirstOrDefaultAsync(c => c.CityId == cityId);
            if (entityToGet == null)
            {
                throw new BusinessException("Not found");
            }

            return mapper.Map<CityOutputViewModel>(entityToGet);
        }

        private IQueryable<City> FilterCities(IQueryable<City> query, CityFilterViewModel queryObject)
        {
            if (queryObject.CityId.HasValue)
            {
                query = query.Where(c => c.CityId == queryObject.CityId);
            }
            if (queryObject.GovernorateId.HasValue)
            {
                query = query.Where(c => c.GovernorateId == queryObject.GovernorateId);
            }
            if (!string.IsNullOrEmpty(queryObject.SearchText))
            {
                query = query.Where(c => _userIdentity.Language == Language.en ? c.CityNameEn.Contains(queryObject.SearchText) :
                _userIdentity.Language == Language.ar ? c.CityNameAr.Contains(queryObject.SearchText) :
                c.CityNameAr.Contains(queryObject.SearchText) || c.CityNameEn.Contains(queryObject.SearchText));
            }

            return query;
        }
    }
}
