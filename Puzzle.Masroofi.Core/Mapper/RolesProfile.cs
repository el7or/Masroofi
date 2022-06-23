using AutoMapper;
using Puzzle.Masroofi.Core.Models;
using Puzzle.Masroofi.Core.ViewModels.Roles;
using Puzzle.Masroofi.Core.ViewModels.UserRoles;

namespace Puzzle.Masroofi.Core.Mapper
{
    public class RolesProfile : Profile
    {
        public RolesProfile()
        {
            CreateMap<Role, AddEditRoleViewModel>().ReverseMap();
            CreateMap<Role, RoleInfoViewModel>()
                .ForMember(dest => dest.PagesCount, cfg => cfg.MapFrom(src => src.ActionsInRoles.Count))
                .ForMember(dest => dest.UsersCount, cfg => cfg.MapFrom(src => src.UserRoles.Count));
            CreateMap<Role, CurrentRoleViewModel>();

            CreateMap<UserRole, AddEditUserRoleViewModel>().ReverseMap();
            CreateMap<UserRole, UserRoleInfoViewModel>();
        }
    }
}
