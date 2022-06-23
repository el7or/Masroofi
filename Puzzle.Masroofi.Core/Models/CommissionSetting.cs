using Puzzle.Masroofi.Core.Enums;
using System;

namespace Puzzle.Masroofi.Core.Models
{
    public partial class CommissionSetting
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
        public Guid CreationUser { get; set; }
        public DateTime? ModificationDate { get; set; }
        public Guid? ModificationUser { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }
}
