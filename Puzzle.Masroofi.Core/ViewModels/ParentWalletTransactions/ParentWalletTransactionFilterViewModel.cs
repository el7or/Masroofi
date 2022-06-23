using System;

namespace Puzzle.Masroofi.Core.ViewModels.ParentWalletTransactions
{
    public class ParentWalletTransactionFilterViewModel : PagedInput
    {
        public Guid? ParentId { get; set; }
        public DateTime? FromCreationDate { get; set; }
        public DateTime? ToCreationDate { get; set; }
        public bool? IsSuccess { get; set; }
        public bool? IsActive { get; set; }
    }
}
