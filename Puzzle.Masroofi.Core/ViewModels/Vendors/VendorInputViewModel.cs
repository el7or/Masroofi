using System;

namespace Puzzle.Masroofi.Core.ViewModels.Vendors
{
    public class VendorInputViewModel
    {
        public Guid? VendorId { get; set; }
        public string FullNameAr { get; set; }
        public string FullNameEn { get; set; }
        public string Phone1 { get; set; }
        public string Phone2 { get; set; }
        public string ResponsiblePerson { get; set; }
        public string Email { get; set; }
        public int CityId { get; set; }
        public string Address { get; set; }
        public string GoogleLocation { get; set; }
        public string ImageUrl { get; set; }
        public PuzzleFileInfo NewImage { get; set; }
    }
}
