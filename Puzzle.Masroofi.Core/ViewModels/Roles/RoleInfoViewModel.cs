using System;

namespace Puzzle.Masroofi.Core.ViewModels.Roles
{
    public class RoleInfoViewModel
    {
        public Guid RoleId { get; set; }
        public string RoleArabicName { get; set; }
        public string RoleEnglishName { get; set; }
        public int? PagesCount { get; set; }
        public int? UsersCount { get; set; }
    }
}
