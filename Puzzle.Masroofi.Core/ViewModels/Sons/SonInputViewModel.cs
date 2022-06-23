using Puzzle.Masroofi.Core.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace Puzzle.Masroofi.Core.ViewModels.Sons
{
    public class SonInputViewModel
    {
        public Guid? SonId { get; set; }
        public Guid ParentId { get; set; }
        public string SonNameAr { get; set; }
        public string SonNameEn { get; set; }
        public int Gender { get; set; }

        [DataType(DataType.Date)]
        public DateTime Birthdate { get; set; }
        public decimal DailyLimit { get; set; }
        public string Mobile { get; set; }
        public string ImageUrl { get; set; }
        public PuzzleFileInfo NewImage { get; set; }
    }
}
