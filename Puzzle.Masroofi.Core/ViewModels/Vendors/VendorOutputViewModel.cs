using System;

namespace Puzzle.Masroofi.Core.ViewModels.Vendors
{
    public class VendorOutputViewModel
    {
        public Guid VendorId { get; set; }
        public string FullNameAr { get; set; }
        public string FullNameEn { get; set; }
        public string FullName { get; set; }
        public string Phone1 { get; set; }
        public string Phone2 { get; set; }
        public string ResponsiblePerson { get; set; }
        public string Email { get; set; }
        public int CityId { get; set; }
        public string Address { get; set; }
        public string GoogleLocation { get; set; }
        public decimal CurrentBalance { get; set; }
        public string ImageUrl { get; set; }
        public DateTime CreationDate { get; set; }
        public bool IsActive { get; set; }
    }
}
