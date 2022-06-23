using Puzzle.Masroofi.Core.Enums;
using System;
using System.Collections.Generic;

namespace Puzzle.Masroofi.Core.Models
{
    public partial class Parent
    {
        public Parent()
        {
            Sons = new HashSet<Son>();
            ParentWalletTransactions = new HashSet<ParentWalletTransaction>();
            ATMCardTransactions = new HashSet<ATMCardTransaction>();
            ParentLoginHistories = new HashSet<ParentLoginHistory>();
            ParentPinCodeHistories = new HashSet<ParentPinCodeHistory>();
            POSMachineTransactions = new HashSet<POSMachineTransaction>();
            ParentMobileRegistrations = new HashSet<ParentMobileRegistration>();
            NotificationParents = new HashSet<ParentNotification>();
        }
        public Guid ParentId { get; set; }
        public string Phone { get; set; }
        public string PinCode { get; set; }
        public long WalletNumber { get; set; }
        public string FullNameAr { get; set; }
        public string FullNameEn { get; set; }
        public string Email { get; set; }
        public int? CityId { get; set; }
        public string Address { get; set; }
        public Gender Gender { get; set; }
        public DateTime? Birthdate { get; set; }
        public decimal CurrentBalance { get; set; }
        public string ImageUrl { get; set; }
        public DateTime CreationDate { get; set; }
        public Guid CreationUser { get; set; }
        public DateTime? ModificationDate { get; set; }
        public Guid? ModificationUser { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public byte[] Timestamp { get; set; }

        public virtual City City { get; set; }
        public virtual ICollection<Son> Sons { get; set; }
        public virtual ICollection<ParentWalletTransaction> ParentWalletTransactions { get; set; }
        public virtual ICollection<ATMCardTransaction> ATMCardTransactions { get; set; }
        public virtual ICollection<ParentLoginHistory> ParentLoginHistories { get; set; }
        public virtual ICollection<ParentPinCodeHistory> ParentPinCodeHistories { get; set; }
        public virtual ICollection<POSMachineTransaction> POSMachineTransactions { get; set; }
        public virtual ICollection<ParentMobileRegistration> ParentMobileRegistrations { get; set; }
        public virtual ICollection<ParentNotification> NotificationParents { get; set; }
    }
}
