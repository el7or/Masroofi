using System;
using System.Collections.Generic;

namespace Puzzle.Masroofi.Core.Models
{
    public partial class POSMachine
    {
        public POSMachine()
        {
            POSMachinePinCodeHistories = new HashSet<POSMachinePinCodeHistory>();
            POSMachineLoginHistories = new HashSet<POSMachineLoginHistory>();
            POSMachineTransactions = new HashSet<POSMachineTransaction>();
            TransactionCommissions = new HashSet<TransactionCommission>();
            ParentWalletTransactions = new HashSet<ParentWalletTransaction>();
            ATMCardTransactions = new HashSet<ATMCardTransaction>();
        }
        public Guid POSMachineId { get; set; }
        public Guid VendorId { get; set; }
        public string Username { get; set; }
        public string PinCode { get; set; }
        public string POSModel { get; set; }
        public string POSNumber { get; set; }
        public DateTime CreationDate { get; set; }
        public Guid CreationUser { get; set; }
        public DateTime? ModificationDate { get; set; }
        public Guid? ModificationUser { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        public virtual Vendor Vendor { get; set; }
        public virtual ICollection<POSMachinePinCodeHistory> POSMachinePinCodeHistories { get; set; }
        public virtual ICollection<POSMachineLoginHistory> POSMachineLoginHistories { get; set; }
        public virtual ICollection<POSMachineTransaction> POSMachineTransactions { get; set; }
        public virtual ICollection<TransactionCommission> TransactionCommissions { get; set; }
        public virtual ICollection<ParentWalletTransaction> ParentWalletTransactions { get; set; }
        public virtual ICollection<ATMCardTransaction> ATMCardTransactions { get; set; }
    }
}
