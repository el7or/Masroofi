using Puzzle.Masroofi.Core.Enums;
using System;

namespace Puzzle.Masroofi.Core.ViewModels.Commissions
{
    public class CommissionOutputViewModel
    {
        public Guid CommissionSettingId { get; set; }
        public decimal FromValue { get; set; }
        public decimal ToValue { get; set; }
        public ChargeParentWalletCommissionType CommissionType { get; set; }
        public decimal FixedCommission { get; set; }
        public int PercentageCommission { get; set; }
        public decimal VendorFixedCommission { get; set; }
        public int VendorPercentageCommission { get; set; }
        public DateTime CreationDate { get; set; }
        public bool IsActive { get; set; }
    }
}
