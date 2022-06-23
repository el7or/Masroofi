using System;

namespace Puzzle.Masroofi.Core.Models
{
    public partial class ActionsInRoles
    {
        public Guid ActionsInRolesId { get; set; }
        public Guid RoleId { get; set; }
        public Guid SystemPageActionId { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? ModificationDate { get; set; }
        public Guid CreationUser { get; set; }
        public Guid? ModificationUser { get; set; }

        public virtual Role Role { get; set; }
        public virtual SystemPageAction SystemPageAction { get; set; }
    }
}
