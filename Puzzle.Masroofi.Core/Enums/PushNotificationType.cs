using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puzzle.Masroofi.Core.Enums
{
    public enum PushNotificationType
    {
        ParentRegistered = 1,
        ChargeBySystem,
        ChargeByCreditCard,
        ChargeByVendor,
        ChargeByAdmin,
        WithdrawBySystem,
        WithdrawByAdmin,
        ChargeATMCard,
        RefundFromATMCard,
        PayByATMCardToVendor,
        RefundFromVendorToATMCard,
        RequestNewATMCard,
        ATMCardStatusChanged        
    }

    public enum SendPriority
    {
        High = 1,
        Medium,
        Low
    }
}
