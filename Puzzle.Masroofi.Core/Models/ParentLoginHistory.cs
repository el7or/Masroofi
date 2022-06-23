using System;

namespace Puzzle.Masroofi.Core.Models
{
    public partial class ParentLoginHistory
    {
        public Guid ParentLoginHistoryId { get; set; }
        public Guid ParentId { get; set; }
        public DateTime LoginDate { get; set; }

        public virtual Parent Parent { get; set; }
    }
}
