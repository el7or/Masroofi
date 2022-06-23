using System;
using System.Collections.Generic;

namespace Puzzle.Masroofi.Core.Models
{
    public partial class Role
    {
        public Role()
        {
            UserRoles = new HashSet<UserRole>();
            ActionsInRoles = new HashSet<ActionsInRoles>();
        }

        public Guid RoleId { get; set; }
        public string RoleArabicName { get; set; }
        public string RoleEnglishName { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? ModificationDate { get; set; }
        public Guid CreationUser { get; set; }
        public Guid? ModificationUser { get; set; }

        public virtual ICollection<UserRole> UserRoles { get; set; }
        public virtual ICollection<ActionsInRoles> ActionsInRoles { get; set; }
    }
}
