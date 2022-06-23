using Puzzle.Masroofi.Core.Enums;
using System;

namespace Puzzle.Masroofi.Core.Models
{
    public partial class ParentPinCodeHistory
    {
        public Guid ParentPinCodeHistoryId { get; set; }

        public Guid ParentId { get; set; }
        public string PinCode { get; set; }
        public PinCodeHistoryType HistoryType { get; set; }
        public Guid UpdatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }

        public virtual Parent Parent { get; set; }
    }
}
