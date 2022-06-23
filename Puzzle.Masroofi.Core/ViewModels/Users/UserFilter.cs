using System;

namespace Puzzle.Masroofi.Core.ViewModels.Users
{
    public class UserFilter : PagedInput
	{
		public Guid? RoleId { get; set; }
		public string SearchText { get; set; }
	}
}
