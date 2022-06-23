using System;

namespace Puzzle.Masroofi.Core.Models
{
    public partial class UserRole
    {
        public Guid UserRoleId { get; set; }
        public Guid UserId { get; set; }
        public Guid RoleId { get; set; }
        public DateTime CreationDate { get; set; }
        public Guid CreationUser { get; set; }
        public DateTime? ModificationDate { get; set; }
        public Guid? ModificationUser { get; set; }

        public virtual Role Role { get; set; }
        public virtual User User { get; set; }
    }
}
