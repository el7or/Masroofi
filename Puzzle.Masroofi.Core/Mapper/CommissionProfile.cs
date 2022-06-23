using AutoMapper;
using Puzzle.Masroofi.Core.Models;
using Puzzle.Masroofi.Core.ViewModels.Commissions;

namespace Puzzle.Masroofi.Core.Mapper
{
    public class CommissionProfile : Profile
    {
        public CommissionProfile()
        {
            CreateMap<CommissionInputViewModel, CommissionSetting>()
                .ReverseMap();

            CreateMap<CommissionSetting, CommissionOutputViewModel>();
        }
    }
}
