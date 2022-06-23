using System;
using System.Collections.Generic;

namespace Puzzle.Masroofi.Core.Models
{
    public partial class Vendor
    {
        public Vendor()
        {
            POSMachines = new HashSet<POSMachine>();
            POSMachineTransactions = new HashSet<POSMachineTransaction>();
            ATMCardTransactions = new HashSet<ATMCardTransaction>();
            ParentWalletTransactions = new HashSet<ParentWalletTransaction>();
            TransactionCommissions = new HashSet<TransactionCommission>();
        }
        public Guid VendorId { get; set; }
        public string FullNameAr { get; set; }
        public string FullNameEn { get; set; }
        public string Phone1 { get; set; }
        public string Phone2 { get; set; }
        public string ResponsiblePerson { get; set; }
        public string Email { get; set; }
        public int CityId { get; set; }
        public string Address { get; set; }
        public string GoogleLocation { get; set; }
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
        public virtual ICollection<POSMachine> POSMachines { get; set; }
        public virtual ICollection<POSMachineTransaction> POSMachineTransactions { get; set; }
        public virtual ICollection<ATMCardTransaction> ATMCardTransactions { get; set; }
        public virtual ICollection<ParentWalletTransaction> ParentWalletTransactions { get; set; }
        public virtual ICollection<TransactionCommission> TransactionCommissions { get; set; }
    }
}
