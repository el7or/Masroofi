using System;

namespace Puzzle.Masroofi.Core.ViewModels.UserRoles
{
    public class UserRoleInfoViewModel
    {
        public Guid UserRoleId { get; set; }
        public Guid UserId { get; set; }
        public Guid RoleId { get; set; }
        public DateTime AssignedDate { get; set; }
    }
}
