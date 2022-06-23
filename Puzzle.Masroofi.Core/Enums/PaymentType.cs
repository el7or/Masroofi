using Puzzle.Masroofi.Core.Helpers;
using Puzzle.Masroofi.Core.Resources;

namespace Puzzle.Masroofi.Core.Enums
{
    public enum PaymentType
    {
        [LocalizedDescription("Visa", typeof(EnumResources))]
        Visa = 1,
        [LocalizedDescription("Cash", typeof(EnumResources))]
        Cash = 2,
        [LocalizedDescription("Transfer", typeof(EnumResources))]
        Transfer = 3,
        [LocalizedDescription("Gift", typeof(EnumResources))]
        Gift = 4,
        [LocalizedDescription("Withdraw", typeof(EnumResources))]
        Withdraw = 5
    }
}
