using System;

namespace Puzzle.Masroofi.Core.ViewModels.ParentMobileRegistrations
{
    public class ParentMobileRegistrationFilterViewModel : PagedInput
    {
        public Guid? ParentId { get; set; }
        public string RegisterType { get; set; }
    }
}
