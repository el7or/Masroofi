using Puzzle.Masroofi.Core.ViewModels.Commissions;
using System;

namespace Puzzle.Masroofi.Core.ViewModels.ParentWalletTransactions
{
    public class ChargeByCreditCardViewModel
    {
        public Guid? ParentWalletTransactionId { get; set; }
        public Guid ParentId { get; set; }
        public decimal Amount { get; set; }
        public decimal FixedCommissionValue { get; set; }
        public decimal PercentageCommissionValue { get; set; }
    }
}
