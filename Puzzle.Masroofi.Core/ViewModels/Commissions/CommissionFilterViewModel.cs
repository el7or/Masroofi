using Puzzle.Masroofi.Core.Enums;

namespace Puzzle.Masroofi.Core.ViewModels.Commissions
{
    public class CommissionFilterViewModel : PagedInput
    {
        public ChargeParentWalletCommissionType? CommissionType { get; set; }
        public decimal? FixedCommission { get; set; }
        public int? PercentageCommission { get; set; }
        public decimal? VendorFixedCommission { get; set; }
        public int? VendorPercentageCommission { get; set; }
    }
}
