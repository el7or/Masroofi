using System;

namespace Puzzle.Masroofi.Core.ViewModels.ATMCardTransactions
{
    public class ATMCardTransactionFilterViewModel : PagedInput
    {
        public Guid? SonId { get; set; }
        public DateTime? FromCreationDate { get; set; }
        public DateTime? ToCreationDate { get; set; }
        public bool? IsSuccess { get; set; }
    }
}
