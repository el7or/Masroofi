using System.Collections.Generic;

namespace Puzzle.Masroofi.Core.ViewModels
{
    public class PagedOutput<T>
    {
        public int TotalCount { get; set; }
        public List<T> Result { get; set; }
    }
}
