using AutoMapper;
using Puzzle.Masroofi.Core.Enums;
using Puzzle.Masroofi.Core.Models;
using Puzzle.Masroofi.Core.ViewModels.Auth;
using Puzzle.Masroofi.Core.ViewModels.Cities;

namespace Puzzle.Masroofi.Core.Mapper
{
    public class CityProfile : Profile
    {
        public CityProfile()
        {
            CreateMap<City, CityOutputViewModel>()
                .AfterMap<MapCityName>();
        }
    }
    public class MapCityName : IMappingAction<City, CityOutputViewModel>
    {
        private readonly UserIdentity _userIdentity;
        public MapCityName(UserIdentity userIdentity)
        {
            _userIdentity = userIdentity;
        }
        public void Process(City source, CityOutputViewModel destination, ResolutionContext context)
        {
            destination.CityName = _userIdentity.Language == Language.en ? source.CityNameEn : source.CityNameAr;
        }
    }
}
