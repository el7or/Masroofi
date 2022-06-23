using System;

namespace Puzzle.Masroofi.Core.ViewModels.POSMachines
{
    public class POSMachinePinCodeViewModel
    {
        public Guid POSMachineId { get; set; }
        public string OldPinCode { get; set; }
        public string PinCode { get; set; }
    }
}
