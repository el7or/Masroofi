using System;

namespace Puzzle.Masroofi.Core.ViewModels.Commissions
{
    public class CommissionValueOutputViewModel
    {
        public Guid? CommissionSettingId { get; set; }
        public decimal Amount { get; set; }
        public decimal FixedCommissionValue { get; set; }
        public decimal PercentageCommissionValue { get; set; }
        public decimal TotalCommissionValues { get; set; }
        public decimal AmountPlusCommissions { get; set; }
    }    
}
