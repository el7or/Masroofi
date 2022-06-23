using AutoMapper;
using Puzzle.Masroofi.Core.Models;
using Puzzle.Masroofi.Core.ViewModels.Users;

namespace Puzzle.Masroofi.Core.Mapper
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserViewModel>().ReverseMap();
            CreateMap<UserInfoViewModel, User>().ReverseMap();
        }
    }
}
