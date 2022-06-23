using Puzzle.Masroofi.Core.ViewModels.Roles;
using System;

namespace Puzzle.Masroofi.Core.ViewModels.Users
{
	public class UserList
	{
		public Guid UserId { get; set; }
		public string NameAr { get; set; }
		public string NameEn { get; set; }
		public string Phone { get; set; }
		public string Email { get; set; }
		public string Username { get; set; }
		public bool? IsActive { get; set; }
		public bool? IsSelected { get; set; }
		public CurrentRoleViewModel CurrentRole { get; set; }
	}
}
