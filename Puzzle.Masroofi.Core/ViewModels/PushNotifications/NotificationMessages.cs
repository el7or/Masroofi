using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puzzle.Masroofi.Core.ViewModels.PushNotifications
{
    public class NotificationMessages
    {
        //General notification title
        public string TitleAr { get; set; }
        public string TitleEn { get; set; }

        public string ParentRegisteredAr { get; set; }
        public string ParentRegisteredEn { get; set; }

        public string ChargeBySystemAr { get; set; }
        public string ChargeBySystemEn { get; set; }

        public string ChargeByCreditCardAr { get; set; }
        public string ChargeByCreditCardEn { get; set; }

        public string ChargeByVendorAr { get; set; }
        public string ChargeByVendorEn { get; set; }

        public string ChargeByAdminAr { get; set; }
        public string ChargeByAdminEn { get; set; }

        public string WithdrawBySystemAr { get; set; }
        public string WithdrawBySystemEn { get; set; }

        public string WithdrawByAdminAr { get; set; }
        public string WithdrawByAdminEn { get; set; }

        public string ChargeATMCardAr { get; set; }
        public string ChargeATMCardEn { get; set; }

        public string RefundFromATMCardAr { get; set; }
        public string RefundFromATMCardEn { get; set; }

        public string PayByATMCardToVendorAr { get; set; }
        public string PayByATMCardToVendorEn { get; set; }

        public string RefundFromVendorToATMCardAr { get; set; }
        public string RefundFromVendorToATMCardEn { get; set; }

        public string RequestNewATMCardAr { get; set; }
        public string RequestNewATMCardEn { get; set; }

        public string ATMCardStatusChangedAr { get; set; }
        public string ATMCardStatusChangedEn { get; set; }
    }
}
