using AutoMapper;
using Puzzle.Masroofi.Core.Enums;
using Puzzle.Masroofi.Core.Models;
using Puzzle.Masroofi.Core.ViewModels.Auth;
using Puzzle.Masroofi.Core.ViewModels.Parents;
using Puzzle.Masroofi.Core.ViewModels.ParentWalletTransactions;

namespace Puzzle.Masroofi.Core.Mapper
{
    public class ParentProfile : Profile
    {
        public ParentProfile()
        {
            CreateMap<ParentInputViewModel, Parent>()
                .ForMember(dest => dest.WalletNumber, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<Parent, ParentOutputViewModel>()
                .ForMember(dest => dest.SonsCount, cfg => cfg.MapFrom(src => src.Sons.Count))
                .ForMember(dest => dest.GovernorateId, cfg => cfg.MapFrom(src => src.City.GovernorateId))
                .AfterMap<MapParentName>();

            CreateMap<ParentWalletTransaction, ParentWalletTransactionOutputViewModel>();
        }
    }
    public class MapParentName : IMappingAction<Parent, ParentOutputViewModel>
    {
        private readonly UserIdentity _userIdentity;
        public MapParentName(UserIdentity userIdentity)
        {
            _userIdentity = userIdentity;
        }
        public void Process(Parent source, ParentOutputViewModel destination, ResolutionContext context)
        {
            destination.FullName = _userIdentity.Language == Language.en ? source.FullNameEn : source.FullNameAr;
            if (destination.CityId > 0)
            {
                destination.CityName = _userIdentity.Language == Language.en ? source.City.CityNameEn : source.City.CityNameAr;
                destination.GovernorateName = _userIdentity.Language == Language.en ? source.City.Governorate.GovernorateNameEn : source.City.Governorate.GovernorateNameAr;
            }
        }
    }
}
