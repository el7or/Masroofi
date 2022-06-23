using AutoMapper;
using Puzzle.Masroofi.Core.Models;
using Puzzle.Masroofi.Core.ViewModels.RoleActions;
using Puzzle.Masroofi.Core.ViewModels.SystemPageActions;
using Puzzle.Masroofi.Core.ViewModels.SystemPages;
using System.Linq;

namespace Puzzle.Masroofi.Core.Mapper
{
    public class SystemPagesProfile : Profile
    {
        public SystemPagesProfile()
        {
            CreateMap<SystemPage, AddEditSystemPageViewModel>().ReverseMap();
            CreateMap<SystemPage, SystemPageInfoViewModel>();

            CreateMap<SystemPageAction, AddEditSystemPageActionViewModel>().ReverseMap();
            CreateMap<SystemPageAction, SystemPageActionInfoViewModel>()
                .ForMember(dest => dest.IsSelected, cfg => cfg.MapFrom(src => src.ActionsInRoles.Any()));


            CreateMap<ActionsInRoles, AddEditRoleActionViewModel>().ReverseMap();
            CreateMap<ActionsInRoles, RoleActionInfoViewModel>();
        }
    }
}
