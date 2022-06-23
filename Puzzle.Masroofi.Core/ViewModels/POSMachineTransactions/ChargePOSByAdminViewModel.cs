using System;

namespace Puzzle.Masroofi.Core.ViewModels.POSMachineTransactions
{
    public class ChargePOSByAdminViewModel
    {
        public Guid VendorId { get; set; }
        public Guid POSMachineId { get; set; }
        public decimal Amount { get; set; }
    }
}
