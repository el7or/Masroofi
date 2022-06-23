using System;

namespace Puzzle.Masroofi.Core.ViewModels.UserRoles
{
    public class UserRoleFilterByTextInputViewModel : PagedInput
    {
        public Guid UserId { get; set; }
        public Guid RoleId { get; set; }
        public string Text { get; set; }
    }
}
