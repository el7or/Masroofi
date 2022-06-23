using AutoMapper;
using Puzzle.Masroofi.Core.Enums;
using Puzzle.Masroofi.Core.Models;
using Puzzle.Masroofi.Core.ViewModels.Auth;
using Puzzle.Masroofi.Core.ViewModels.Governorates;

namespace Puzzle.Masroofi.Core.Mapper
{
    public class GovernorateProfile : Profile
    {
        public GovernorateProfile()
        {
            CreateMap<Governorate, GovernorateOutputViewModel>()
                .AfterMap<MapGovernorateName>();
        }
    }
    public class MapGovernorateName : IMappingAction<Governorate, GovernorateOutputViewModel>
    {
        private readonly UserIdentity _userIdentity;
        public MapGovernorateName(UserIdentity userIdentity)
        {
            _userIdentity = userIdentity;
        }
        public void Process(Governorate source, GovernorateOutputViewModel destination, ResolutionContext context)
        {
            destination.GovernorateName = _userIdentity.Language == Language.en ? source.GovernorateNameEn : source.GovernorateNameAr;
        }
    }
}
