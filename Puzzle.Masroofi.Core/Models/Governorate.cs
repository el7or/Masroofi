using System.Collections.Generic;

namespace Puzzle.Masroofi.Core.Models
{
    public partial class Governorate
    {
        public Governorate()
        {
            Cities = new HashSet<City>();
        }
        public int GovernorateId { get; set; }
        public string GovernorateNameAr { get; set; }
        public string GovernorateNameEn { get; set; }

        public virtual ICollection<City> Cities { get; set; }
    }
}
