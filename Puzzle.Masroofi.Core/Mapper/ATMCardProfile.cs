using AutoMapper;
using Puzzle.Masroofi.Core.Enums;
using Puzzle.Masroofi.Core.Models;
using Puzzle.Masroofi.Core.ViewModels.Auth;
using Puzzle.Masroofi.Core.ViewModels.ATMCards;
using Puzzle.Masroofi.Core.Extensions;
using System.Linq;
using Puzzle.Masroofi.Core.ViewModels.Sons;
using System.Collections.Generic;

namespace Puzzle.Masroofi.Core.Mapper
{
    public class ATMCardProfile : Profile
    {
        
        public ATMCardProfile()
        {
            CreateMap<ATMCardInputViewModel, ATMCard>()
                .ReverseMap();


            CreateMap<ATMCard, ATMCardSonOutput>().ForMember(dest => dest.StatusText, cfg => cfg.MapFrom(src => src.Status.GetDescription()))
                .ForMember(dest => dest.IsCurrentATMCard, cfg => cfg.MapFrom(src => src.Son.CurrentATMCardId == src.ATMCardId))
                .ForMember(dest => dest.ExpiryMonth, cfg => cfg.MapFrom(src => src.ExpiryDate.HasValue ? src.ExpiryDate.Value.Month.ToString() : ""))
                .ForMember(dest => dest.ExpiryYear, cfg => cfg.MapFrom(src => src.ExpiryDate.HasValue ? src.ExpiryDate.Value.Year.ToString() : ""))
                .AfterMap<MapATMCardSonName>();

            CreateMap<ATMCard, ATMCardOutputViewModel>()
                .ForMember(dest => dest.StatusText, cfg => cfg.MapFrom(src => src.Status.GetDescription()))
                .ForMember(dest => dest.IsCurrentATMCard, cfg => cfg.MapFrom(src => src.Son.CurrentATMCardId == src.ATMCardId))
                .ForMember(dest => dest.ExpiryMonth, cfg => cfg.MapFrom(src => src.ExpiryDate.HasValue ? src.ExpiryDate.Value.Month.ToString() : ""))
                .ForMember(dest => dest.ExpiryYear, cfg => cfg.MapFrom(src => src.ExpiryDate.HasValue ? src.ExpiryDate.Value.Year.ToString() : ""));
                
                
        }
       
    }
    public class MapATMCardSonName : IMappingAction<ATMCard, ATMCardSonOutput>
    {
        private readonly UserIdentity _userIdentity;
        public MapATMCardSonName(UserIdentity userIdentity)
        {
            _userIdentity = userIdentity;
        }
        public void Process(ATMCard source, ATMCardSonOutput destination, ResolutionContext context)
        {
           
                destination.SonName = _userIdentity.Language == Language.en ? source.Son.SonNameEn : source.Son.SonNameAr;
                destination.ParentName = _userIdentity.Language == Language.en ? source.Son.Parent.FullNameEn : source.Son.Parent.FullNameAr;
              
            
           
          
        }

    }

}
