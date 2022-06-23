using AutoMapper;
using Puzzle.Masroofi.Core.Models;
using Puzzle.Masroofi.Core.ViewModels.ATMCardTypes;
using Puzzle.Masroofi.Core.ViewModels.Auth;
using Puzzle.Masroofi.Core.Enums;

namespace Puzzle.Masroofi.Core.Mapper
{
    public class ATMCardTypeProfile : Profile
    {
        public ATMCardTypeProfile()
        {
            CreateMap<ATMCardTypeInputViewModel, ATMCardType>()
                .ReverseMap();

            CreateMap<ATMCardType, ATMCardTypeOutputViewModel>().AfterMap<MapATMCardTypeName>(); ;
        }
    }
    public class MapATMCardTypeName : IMappingAction<ATMCardType, ATMCardTypeOutputViewModel>
    {
        private readonly UserIdentity _userIdentity;
        public MapATMCardTypeName(UserIdentity userIdentity)
        {
            _userIdentity = userIdentity;
        }
        public void Process(ATMCardType source, ATMCardTypeOutputViewModel destination, ResolutionContext context)
        {
            destination.TypeName = _userIdentity.Language == Language.en ? source.TypeNameEn : source.TypeNameAr;
        }
    }
}
