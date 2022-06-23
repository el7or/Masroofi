
namespace Puzzle.Masroofi.Core.ViewModels
{
    public class PagedInput
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string SortBy { get; set; }
        public bool IsSortAscending { get; set; }
    }
}
