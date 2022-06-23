using Puzzle.Masroofi.Core.Enums;
using System;

namespace Puzzle.Masroofi.Core.Models
{
    public partial class TransactionCommission
    {
        public Guid TransactionCommissionId { get; set; }
        public Guid ParentWalletTransactionId { get; set; }
        public Guid? POSMachineTransactionId { get; set; }
        public Guid? VendorId { get; set; }
        public Guid? POSMachineId { get; set; }
        public decimal TransactionValue { get; set; }
        public ChargeParentWalletCommissionType TransactionType { get; set; }
        public decimal FixedCommissionValue { get; set; }
        public decimal PercentageCommissionValue { get; set; }
        public decimal? VendorFixedCommissionValue { get; set; }
        public decimal? VendorPercentageCommissionValue { get; set; }
        public DateTime CreationDate { get; set; }

        public virtual ParentWalletTransaction ParentWalletTransaction { get; set; }
        public virtual POSMachineTransaction POSMachineTransaction { get; set; }
        public virtual Vendor Vendor { get; set; }
        public virtual POSMachine POSMachine { get; set; }
    }
}
