using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puzzle.Masroofi.Core.ViewModels.ParentMobileRegistrations
{
    public class ParentMobileRegistrationInputViewModel
    {
        public Guid? ParentMobileRegistrationId { get; set; }
        public Guid ParentId { get; set; }
        public string RegisterId { get; set; }
        public string RegisterType { get; set; }
    }
}
