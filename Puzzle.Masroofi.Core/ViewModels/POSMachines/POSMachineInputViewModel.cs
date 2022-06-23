using System;

namespace Puzzle.Masroofi.Core.ViewModels.POSMachines
{
    public class POSMachineInputViewModel
    {
        public Guid? POSMachineId { get; set; }
        public Guid VendorId { get; set; }
        public string Username { get; set; }
        public string PinCode { get; set; }
        public string POSModel { get; set; }
        public string POSNumber { get; set; }
    }
}
