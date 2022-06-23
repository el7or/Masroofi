using Puzzle.Masroofi.Core.Enums;
using Puzzle.Masroofi.Core.ViewModels.ATMCards;
using Puzzle.Masroofi.Core.ViewModels.Parents;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Puzzle.Masroofi.Core.ViewModels.Sons
{
    public class SonOutputViewModel
    {
        public Guid SonId { get; set; }
        public Guid ParentId { get; set; }
        public string SonNameAr { get; set; }
        public string SonNameEn { get; set; }
        public string SonName { get; set; }
        public int Gender { get; set; }

        [DataType(DataType.Date)]
        public DateTime Birthdate { get; set; }
        public decimal DailyLimit { get; set; }
        public decimal CurrentBalance { get; set; }
        public string ImageUrl { get; set; }
        public Guid? CurrentATMCardId { get; set; }
        public string Mobile { get; set; }
        public DateTime CreationDate { get; set; }
        public bool IsActive { get; set; }

        

    public List<ATMCardOutputViewModel> ATMCards { get; set; }
    }
}
