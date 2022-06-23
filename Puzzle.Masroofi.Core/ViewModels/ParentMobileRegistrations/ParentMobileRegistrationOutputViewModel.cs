using System;

namespace Puzzle.Masroofi.Core.ViewModels.ParentMobileRegistrations
{
    public class ParentMobileRegistrationOutputViewModel
    {
        public Guid ParentMobileRegistrationId { get; set; }
        public Guid ParentId { get; set; }
        public string RegisterId { get; set; }
        public string RegisterType { get; set; }
        public DateTime CreationDate { get; set; }
        public bool IsActive { get; set; }
    }
}
