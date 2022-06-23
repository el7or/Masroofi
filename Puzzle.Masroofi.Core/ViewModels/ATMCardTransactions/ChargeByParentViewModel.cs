using System;

namespace Puzzle.Masroofi.Core.ViewModels.ATMCardTransactions
{
    public class ChargeByParentViewModel
    {
        public Guid ATMCardId { get; set; }
        public Guid SonId { get; set; }
        public Guid ParentId { get; set; }
        public decimal Amount { get; set; }
    }
}
