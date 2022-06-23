using AutoMapper;
using Puzzle.Masroofi.Core.Enums;
using Puzzle.Masroofi.Core.Models;
using Puzzle.Masroofi.Core.ViewModels.Auth;
using Puzzle.Masroofi.Core.ViewModels.Vendors;

namespace Puzzle.Masroofi.Core.Mapper
{
    public class VendorProfile : Profile
    {
        public VendorProfile()
        {
            CreateMap<VendorInputViewModel, Vendor>()
                .ReverseMap();

            CreateMap<Vendor, VendorOutputViewModel>()
                .AfterMap<MapVendorName>();
        }
    }
    public class MapVendorName : IMappingAction<Vendor, VendorOutputViewModel>
    {
        private readonly UserIdentity _userIdentity;
        public MapVendorName(UserIdentity userIdentity)
        {
            _userIdentity = userIdentity;
        }
        public void Process(Vendor source, VendorOutputViewModel destination, ResolutionContext context)
        {
            destination.FullName = _userIdentity.Language == Language.en ? source.FullNameEn : source.FullNameAr;
        }
    }
}
