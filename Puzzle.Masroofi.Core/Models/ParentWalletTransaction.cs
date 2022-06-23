using Puzzle.Masroofi.Core.Enums;
using System;
using System.Collections.Generic;

namespace Puzzle.Masroofi.Core.Models
{
    public partial class ParentWalletTransaction
    {
        public ParentWalletTransaction()
        {
            TransactionCommissions = new HashSet<TransactionCommission>();
        }
        public Guid ParentWalletTransactionId { get; set; }
        public Guid ParentId { get; set; }
        public Guid? SonId { get; set; }
        public Guid? ATMCardId { get; set; }
        public Guid? VendorId { get; set; }
        public Guid? POSMachineId { get; set; }
        public Guid? AdminUserId { get; set; }
        public long TransactionNumber { get; set; }
        public decimal Amount { get; set; }
        public PaymentType PaymentType { get; set; }
        public string TitleAr { get; set; }
        public string TitleEn { get; set; }
        public string DetailsAr { get; set; }
        public string DetailsEn { get; set; }
        public string TransactionReference { get; set; }
        public string TransactionDataJson { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? ModificationDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsSuccess { get; set; }

        public virtual Parent Parent { get; set; }
        public virtual Son Son { get; set; }
        public virtual Vendor Vendor { get; set; }
        public virtual POSMachine POSMachine { get; set; }
        public virtual ATMCard ATMCard { get; set; }
        public virtual User AdminUser { get; set; }
        public virtual ICollection<TransactionCommission> TransactionCommissions { get; set; }
    }
}
