using AutoMapper;
using Puzzle.Masroofi.Core.Models;
using Puzzle.Masroofi.Core.ViewModels.ParentMobileRegistrations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puzzle.Masroofi.Core.Mapper
{
    public class ParentMobileRegistrationProfile : Profile
    {
        public ParentMobileRegistrationProfile()
        {
            CreateMap<ParentMobileRegistrationInputViewModel, ParentMobileRegistration>()
                .ReverseMap();

            CreateMap<ParentMobileRegistration, ParentMobileRegistrationOutputViewModel>();
        }
    }
}
