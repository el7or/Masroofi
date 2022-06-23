using AutoMapper;
using Puzzle.Masroofi.Core.Enums;
using Puzzle.Masroofi.Core.Models;
using Puzzle.Masroofi.Core.ViewModels.Auth;
using Puzzle.Masroofi.Core.ViewModels.Sons;
using System.Collections.Generic;
using System.Linq;

namespace Puzzle.Masroofi.Core.Mapper
{
    public class SonProfile : Profile
    {
        public SonProfile()
        {
            CreateMap<SonInputViewModel, Son>()
                .ReverseMap();

            CreateMap<Son, SonOutputViewModel>().PreserveReferences()
                .AfterMap<MapSonName>();

       
        }
    }
    public class MapSonName : IMappingAction<Son, SonOutputViewModel>
    {
        private readonly UserIdentity _userIdentity;
        public MapSonName(UserIdentity userIdentity)
        {
            _userIdentity = userIdentity;
        }
        public void Process(Son source, SonOutputViewModel destination, ResolutionContext context)
        {
            destination.SonName = _userIdentity.Language == Language.en ? source.SonNameEn : source.SonNameAr;
        }
    }
  
}
