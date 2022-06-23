using System;

namespace Puzzle.Masroofi.Core.ViewModels.POSMachines
{
    public class POSMachineFilterViewModel : PagedInput
    {
        public Guid? VendorId { get; set; }
        public string Username { get; set; }
        public string POSModel { get; set; }
        public string POSNumber { get; set; }
    }
}
