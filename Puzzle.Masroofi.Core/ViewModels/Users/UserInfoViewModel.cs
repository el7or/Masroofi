using System;

namespace Puzzle.Masroofi.Core.ViewModels.Users
{
    public class UserInfoViewModel
    {
		public Guid UserId { get; set; }
		public string Username { get; set; }
		public string Password { get; set; }
		public string NameAr { get; set; }
		public string NameEn { get; set; }
		public string Email { get; set; }
		public string Phone { get; set; }
		public bool? IsActive { get; set; }
		public int? UserType { get; set; }
		public string Image { get; set; }
	}
}
