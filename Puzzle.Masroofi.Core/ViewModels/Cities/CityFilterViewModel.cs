namespace Puzzle.Masroofi.Core.ViewModels.Cities
{
    public class CityFilterViewModel : PagedInput
    {
        public int? CityId { get; set; }
        public int? GovernorateId { get; set; }
        public string SearchText { get; set; }
    }
}
