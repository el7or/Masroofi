using System;

namespace Puzzle.Masroofi.Core.ViewModels.ATMCardTransactions
{
    public class RefundFromVendorViewModel
    {
        public Guid ATMCardId { get; set; }
        public Guid SonId { get; set; }
        public Guid VendorId { get; set; }
        public Guid POSMachineId { get; set; }
        public string POSMachinePinCode { get; set; }
        public Guid ReferenceTransactionId { get; set; }
        public decimal Amount { get; set; }
    }
}
