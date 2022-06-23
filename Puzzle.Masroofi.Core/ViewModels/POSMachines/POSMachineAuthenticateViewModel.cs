using System;

namespace Puzzle.Masroofi.Core.ViewModels.POSMachines
{
    public class POSMachineAuthenticateViewModel
    {
        public object Token { get; set; }
        public Guid POSMachineId { get; set; }
        public POSMachineOutputViewModel POSMachineInfo { get; set; }
        public string CurrencySymbol { get; set; }

        public string MinorCurrencySymbol { get; set; }
    }
}
