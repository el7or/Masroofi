using Puzzle.Masroofi.Core.Enums;
using System;

namespace Puzzle.Masroofi.Core.Models
{
    public partial class POSMachinePinCodeHistory
    {
        public Guid POSMachinePinCodeHistoryId { get; set; }

        public Guid POSMachineId { get; set; }
        public string PinCode { get; set; }
        public PinCodeHistoryType HistoryType { get; set; }
        public Guid UpdatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }

        public virtual POSMachine POSMachine { get; set; }
    }
}
