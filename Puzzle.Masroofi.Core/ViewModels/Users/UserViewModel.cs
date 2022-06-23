using System;

namespace Puzzle.Masroofi.Core.ViewModels.Users
{
    public class UserViewModel
	{
		public Guid? UserId { get; set; }
		public string Username { get; set; }
		public string Password { get; set; }
		public string NameAr { get; set; }
		public string NameEn { get; set; }
		public string Email { get; set; }
		public string Phone { get; set; }
		public int? UserType { get; set; }
		public PuzzleFileInfo UserImage { get; set; }
	}
}
