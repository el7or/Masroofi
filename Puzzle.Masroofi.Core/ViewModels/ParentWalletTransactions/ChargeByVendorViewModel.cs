using Puzzle.Masroofi.Core.ViewModels.Commissions;
using System;

namespace Puzzle.Masroofi.Core.ViewModels.ParentWalletTransactions
{
    public class ChargeByVendorViewModel
    {
        public Guid? ParentWalletTransactionId { get; set; }
        public Guid? POSMachineTransactionId { get; set; }
        public Guid? ParentId { get; set; }
        public string ParentPhone { get; set; }
        public Guid VendorId { get; set; }
        public Guid POSMachineId { get; set; }
        public string POSMachinePinCode { get; set; }
        public decimal Amount { get; set; }
        public decimal FixedCommissionValue { get; set; }
        public decimal PercentageCommissionValue { get; set; }
    }
}
