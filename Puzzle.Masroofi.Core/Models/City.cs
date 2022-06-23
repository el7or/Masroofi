namespace Puzzle.Masroofi.Core.Models
{
    public partial class City
    {
        public int CityId { get; set; }
        public int GovernorateId { get; set; }
        public string CityNameAr { get; set; }
        public string CityNameEn { get; set; }

        public virtual Governorate Governorate { get; set; }
    }
}
