using AutoMapper;
using Puzzle.Masroofi.Core.Models;
using Puzzle.Masroofi.Core.ViewModels.POSMachines;

namespace Puzzle.Masroofi.Core.Mapper
{
    public class POSMachineProfile:Profile
    {
        public POSMachineProfile()
        {
            CreateMap<POSMachineInputViewModel, POSMachine>()
                .ReverseMap();

            CreateMap<POSMachine, POSMachineOutputViewModel>();
        }
    }
}
