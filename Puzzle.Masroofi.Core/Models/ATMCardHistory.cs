using Puzzle.Masroofi.Core.Enums;
using System;

namespace Puzzle.Masroofi.Core.Models
{
    public partial class ATMCardHistory
    {
        public Guid ATMCardHistoryId { get; set; }
        public Guid ATMCardId { get; set; }
        public ATMCardHistoryType HistoryType { get; set; }
        public string Password { get; set; }
        public ATMCardStatus Status { get; set; }
        public Guid UpdatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }

        public virtual ATMCard ATMCard { get; set; }
    }
}
