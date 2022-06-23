using System;

namespace Puzzle.Masroofi.Core.ViewModels.RoleActions
{
    public class AddEditRoleActionViewModel
    {
        public Guid? ActionsInRolesId { get; set; }
        public Guid RoleId { get; set; }
        public Guid SystemPageActionId { get; set; }
    }
}
