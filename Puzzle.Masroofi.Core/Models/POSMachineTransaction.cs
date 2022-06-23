using Puzzle.Masroofi.Core.Enums;
using System;
using System.Collections.Generic;

namespace Puzzle.Masroofi.Core.Models
{
    public partial class POSMachineTransaction
    {
        public POSMachineTransaction()
        {
            TransactionCommissions = new HashSet<TransactionCommission>();
            RefundTransactions = new HashSet<POSMachineTransaction>();
        }
        public Guid POSMachineTransactionId { get; set; }
        public Guid VendorId { get; set; }
        public Guid POSMachineId { get; set; }
        public Guid? ParentId { get; set; }
        public Guid? SonId { get; set; }
        public Guid? ATMCardId { get; set; }
        public Guid? AdminUserId { get; set; }
        public Guid? ReferenceTransactionId { get; set; }
        public long TransactionNumber { get; set; }
        public decimal Amount { get; set; }
        public PaymentType PaymentType { get; set; }
        public string TitleAr { get; set; }
        public string TitleEn { get; set; }
        public string DetailsAr { get; set; }
        public string DetailsEn { get; set; }
        public string Note { get; set; }
        public DateTime CreationDate { get; set; }
        public bool IsSuccess { get; set; }

        public virtual Vendor Vendor { get; set; }
        public virtual POSMachine POSMachine { get; set; }
        public virtual Parent Parent { get; set; }
        public virtual Son Son { get; set; }
        public virtual ATMCard ATMCard { get; set; }
        public virtual User AdminUser { get; set; }
        public virtual POSMachineTransaction ReferenceTransaction { get; set; }
        public virtual ICollection<POSMachineTransaction> RefundTransactions { get; set; }
        public virtual ICollection<TransactionCommission> TransactionCommissions { get; set; }
    }
}
