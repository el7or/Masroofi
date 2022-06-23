using System;

namespace Puzzle.Masroofi.Core.ViewModels.Commissions
{
    public class ParentCommissionInputViewModel
    {
        public Guid? ParentId { get; set; }
        public string ParentPhone { get; set; }
        public decimal Amount { get; set; }
    }
}
