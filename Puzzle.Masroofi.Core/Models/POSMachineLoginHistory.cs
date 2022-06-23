using System;

namespace Puzzle.Masroofi.Core.Models
{
    public partial class POSMachineLoginHistory
    {
        public Guid POSMachineLoginHistoryId { get; set; }
        public Guid POSMachineId { get; set; }
        public DateTime LoginDate { get; set; }

        public virtual POSMachine POSMachine { get; set; }
    }
}
