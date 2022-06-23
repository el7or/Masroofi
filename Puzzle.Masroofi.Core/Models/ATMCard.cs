using Puzzle.Masroofi.Core.Enums;
using System;
using System.Collections.Generic;

namespace Puzzle.Masroofi.Core.Models
{
    public partial class ATMCard
    {
        public ATMCard()
        {
            ATMCardHistories = new HashSet<ATMCardHistory>();
            ATMCardTransactions = new HashSet<ATMCardTransaction>();
            ParentWalletTransactions = new HashSet<ParentWalletTransaction>();
            POSMachineTransactions = new HashSet<POSMachineTransaction>();
        }
        public Guid ATMCardId { get; set; }
        public Guid SonId { get; set; }
        public Guid ATMCardTypeId { get; set; }
        public long CardId { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public string ShortNumber { get; set; }
        public string CardNumber { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string SecurityCode { get; set; }
        public ATMCardStatus Status { get; set; }
        public string RejectedReason { get; set; }
        public DateTime CreationDate { get; set; }
        public Guid CreationUser { get; set; }
        public DateTime? ModificationDate { get; set; }
        public Guid? ModificationUser { get; set; }
        public bool IsDeleted { get; set; }
       


        public virtual Son Son { get; set; }
        public virtual ATMCardType ATMCardType { get; set; }
        public virtual ICollection<ATMCardHistory> ATMCardHistories { get; set; }
        public virtual ICollection<ATMCardTransaction> ATMCardTransactions { get; set; }
        public virtual ICollection<ParentWalletTransaction> ParentWalletTransactions { get; set; }
        public virtual ICollection<POSMachineTransaction> POSMachineTransactions { get; set; }
    }
}
