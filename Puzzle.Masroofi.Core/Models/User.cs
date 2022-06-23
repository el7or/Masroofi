using System;
using System.Collections.Generic;

namespace Puzzle.Masroofi.Core.Models
{
    public partial class User
    {
        public User()
        {
            UserRoles = new HashSet<UserRole>();
            ParentWalletTransactions = new HashSet<ParentWalletTransaction>();
            POSMachineTransactions = new HashSet<POSMachineTransaction>();
        }

        public Guid UserId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string NameAr { get; set; }
        public string NameEn { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public bool? Online { get; set; }
        public bool? IsVerified { get; set; }
        public int? UserType { get; set; }
        public string Image { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreationDate { get; set; }
        public Guid CreationUser { get; set; }
        public DateTime? ModificationDate { get; set; }
        public Guid? ModificationUser { get; set; }

        public virtual ICollection<UserRole> UserRoles { get; set; }
        public virtual ICollection<ParentWalletTransaction> ParentWalletTransactions { get; set; }
        public virtual ICollection<POSMachineTransaction> POSMachineTransactions { get; set; }
    }
}
