using Puzzle.Masroofi.Core.Helpers;
using Puzzle.Masroofi.Core.Resources;
using System.ComponentModel;

namespace Puzzle.Masroofi.Core.Enums
{
    public enum ATMCardStatus
    {
        [LocalizedDescription("Pending", typeof(EnumResources))]
        Pending = 1,
        [LocalizedDescription("Rejected", typeof(EnumResources))]
        Rejected = 2,
        [LocalizedDescription("InProgress", typeof(EnumResources))]
        InProgress = 3,
        [LocalizedDescription("Shipping", typeof(EnumResources))]
        Shipping = 4,
        [LocalizedDescription("Received", typeof(EnumResources))]
        Received = 5,
        [LocalizedDescription("Active", typeof(EnumResources))]
        Active = 6,
        [LocalizedDescription("Deactivated", typeof(EnumResources))]
        Deactivated = 7,
        [LocalizedDescription("Lost", typeof(EnumResources))]
        Lost = 8,
        [LocalizedDescription("Replaced", typeof(EnumResources))]
        Replaced = 9
    }
}
