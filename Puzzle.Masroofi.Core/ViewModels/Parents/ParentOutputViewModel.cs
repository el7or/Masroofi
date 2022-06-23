using System;
using System.ComponentModel.DataAnnotations;

namespace Puzzle.Masroofi.Core.ViewModels.Parents
{
    public class ParentOutputViewModel
    {
        public Guid ParentId { get; set; }
        public string Phone { get; set; }
        public string PinCode { get; set; }
        public string WalletNumber { get; set; }
        public string FullNameAr { get; set; }
        public string FullNameEn { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public int? CityId { get; set; }
        public string CityName { get; set; }
        public int? GovernorateId { get; set; }
        public string GovernorateName { get; set; }
        public string Address { get; set; }
        public int Gender { get; set; }

        [DataType(DataType.Date)]
        public DateTime? Birthdate { get; set; }
        public decimal CurrentBalance { get; set; } = 0;
        public string ImageUrl { get; set; }
        public DateTime CreationDate { get; set; }
        public bool IsActive { get; set; }
        public int? SonsCount { get; set; }
    }
}
