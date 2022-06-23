using System;

namespace Puzzle.Masroofi.Core.ViewModels.ParentWalletTransactions
{
    public class WithdrawByAdminViewModel
    {
        public Guid? ParentWalletTransactionId { get; set; }
        public Guid ParentId { get; set; }
        public decimal Amount { get; set; }
    }
}
