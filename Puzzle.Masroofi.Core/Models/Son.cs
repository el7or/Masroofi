using Puzzle.Masroofi.Core.Enums;
using System;
using System.Collections.Generic;

namespace Puzzle.Masroofi.Core.Models
{
    public partial class Son
    {
        public Son()
        {
            ATMCards = new HashSet<ATMCard>();
            ParentWalletTransactions = new HashSet<ParentWalletTransaction>();
            ATMCardTransactions = new HashSet<ATMCardTransaction>();
            POSMachineTransactions = new HashSet<POSMachineTransaction>();
        }
        public Guid SonId { get; set; }
        public Guid ParentId { get; set; }
        public string SonNameAr { get; set; }
        public string SonNameEn { get; set; }
        public Gender Gender { get; set; }
        public DateTime Birthdate { get; set; }
        public decimal DailyLimit { get; set; }
        public decimal CurrentBalance { get; set; }
        public string ImageUrl { get; set; }
        public string Mobile { get; set; }
        public Guid? CurrentATMCardId { get; set; }
        public DateTime CreationDate { get; set; }
        public Guid CreationUser { get; set; }
        public DateTime? ModificationDate { get; set; }
        public Guid? ModificationUser { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public byte[] Timestamp { get; set; }

        public virtual Parent Parent { get; set; }
        public virtual ATMCard CurrentATMCard { get; set; }
        public virtual ICollection<ATMCard> ATMCards { get; set; }
        public virtual ICollection<ParentWalletTransaction> ParentWalletTransactions { get; set; }
        public virtual ICollection<ATMCardTransaction> ATMCardTransactions { get; set; }
        public virtual ICollection<POSMachineTransaction> POSMachineTransactions { get; set; }
    }
}
