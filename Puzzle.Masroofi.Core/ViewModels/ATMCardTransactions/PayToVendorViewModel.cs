using System;

namespace Puzzle.Masroofi.Core.ViewModels.ATMCardTransactions
{
    public class PayToVendorViewModel
    {
        public Guid ATMCardId { get; set; }
        public Guid SonId { get; set; }
        public Guid VendorId { get; set; }
        public Guid POSMachineId { get; set; }
        public decimal Amount { get; set; }
        public string ATMCardPassword { get; set; }
    }
}
