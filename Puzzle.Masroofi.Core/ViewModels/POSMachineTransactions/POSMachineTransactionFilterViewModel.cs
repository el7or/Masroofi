using System;

namespace Puzzle.Masroofi.Core.ViewModels.POSMachineTransactions
{
    public class POSMachineTransactionFilterViewModel : PagedInput
    {
        public Guid? VendorId { get; set; }
        public Guid? POSMachineId { get; set; }
        public Guid? ATMCardId { get; set; }
        public DateTime? FromCreationDate { get; set; }
        public DateTime? ToCreationDate { get; set; }
        public string SonName { get; set; }
        public bool? IsSuccess { get; set; }
    }
}
