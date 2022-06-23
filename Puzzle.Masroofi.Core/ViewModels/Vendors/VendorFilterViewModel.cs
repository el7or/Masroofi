namespace Puzzle.Masroofi.Core.ViewModels.Vendors
{
    public class VendorFilterViewModel : PagedInput
    {
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public int? CityId { get; set; }
    }
}
