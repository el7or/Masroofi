using Puzzle.Masroofi.Core.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace Puzzle.Masroofi.Core.ViewModels.Parents
{
    public class ParentInputViewModel
    {
        public Guid? ParentId { get; set; }
        public string Phone { get; set; }
        public string PinCode { get; set; }
        public string FullNameAr { get; set; }
        public string FullNameEn { get; set; }
        public string Email { get; set; }
        public int? CityId { get; set; }
        public string Address { get; set; }
        public int Gender { get; set; }

        [DataType(DataType.Date)]
        public DateTime? Birthdate { get; set; }
        public string ImageUrl { get; set; }
        public PuzzleFileInfo NewImage { get; set; }
    }
}
