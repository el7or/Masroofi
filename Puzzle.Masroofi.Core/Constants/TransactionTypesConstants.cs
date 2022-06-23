
namespace Puzzle.Masroofi.Core.Constants
{
    public static class TransactionTypesConstants
    {
        public static class ParentWalletTransactionTypes
        {
            /// <summary>
            /// TransactionInfo: Add 20 L.E as a gift for Parent wallet after added first photo for first Son, Only once for every Parent.
            /// <para>TransactionDirection: + in ParentWalletTransaction.</para>
            /// </summary>
            public static class ChargeParentWalletBySystem
            {
                public const string TitleAr = "هدية";
                public const string TitleEn = "Gift";
                public const string DetailsAr = "هدية مقدمة عند إضافة أول صورة لأول إبن";
                public const string DetailsEn = "A gift provided when adding the first photo of the first son.";
            }

            /// <summary>
            /// TransactionInfo: Withdraw From Parent Wallet By System when request new ATMCard for Son
            /// <para>TransactionDirection: - in ParentWalletTransaction</para>
            /// </summary>
            public static class WithdrawFromParentWalletBySystem
            {
                public const string TitleAr = "سحب رصيد";
                public const string TitleEn = "Withdraw";
                public const string DetailsAr = "سحب رصيد من محفظة الوالد بواسطة النظام عند طلب كارت جديد للابن";
                public const string DetailsEn = "Withdraw From Parent Wallet By System when request new ATMCard for Son";
            }

            /// <summary>
            /// TransactionInfo: Charge Parent Wallet By Credit Card
            /// <para>TransactionDirection: + in ParentWalletTransaction</para>
            /// </summary>
            public static class ChargeParentWalletByCreditCard
            {
                public const string TitleAr = "شحن رصيد";
                public const string TitleEn = "Charge";
                public const string DetailsAr = "شحن محفظة الوالد عن طريق بطاقة الائتمان";
                public const string DetailsEn = "Charge Parent Wallet By Credit Card";
            }

            /// <summary>
            /// TransactionInfo: Charge Parent Wallet By Vendor
            /// <para>TransactionDirection: + in ParentWalletTransaction, - in POSMachineTransaction</para>
            /// </summary>
            public static class ChargeParentWalletByVendor
            {
                public const string TitleAr = "شحن رصيد";
                public const string TitleEn = "Charge";
                public const string DetailsAr = "شحن محفظة الوالد عن طريق التاجر";
                public const string DetailsEn = "Charge Parent Wallet By Vendor";
            }

            /// <summary>
            /// TransactionInfo: Charge Parent Wallet By Admin
            /// <para>TransactionDirection: + in ParentWalletTransaction</para>
            /// </summary>
            public static class ChargeParentWalletByAdmin
            {
                public const string TitleAr = "شحن رصيد";
                public const string TitleEn = "Charge";
                public const string DetailsAr = "شحن محفظة الوالد عن طريق الأدمن";
                public const string DetailsEn = "Charge Parent Wallet By Admin";
            }

            /// <summary>
            /// TransactionInfo: Withdraw From Parent Wallet By Admin
            /// <para>TransactionDirection: - in ParentWalletTransaction</para>
            /// </summary>
            public static class WithdrawFromParentWalletByAdmin
            {
                public const string TitleAr = "سحب رصيد";
                public const string TitleEn = "Withdraw";
                public const string DetailsAr = "سحب رصيد من محفظة الوالد بواسطة الأدمن";
                public const string DetailsEn = "Withdraw From Parent Wallet By Admin";
            }
        }

        public static class ATMCardTransactionTypes
        {
            /// <summary>
            /// TransactionInfo: Charge ATMCard By Parent Wallet
            /// <para>TransactionDirection: + in ATMCardTransaction, - in ParentWalletTransaction</para>
            /// </summary>
            public static class ChargeATMCardByParent 
            {
                public const string TitleAr = "شحن رصيد";
                public const string TitleEn = "Charge";
                public const string DetailsAr = "شحن كارت الإبن بواسطة محفظة الوالد";
                public const string DetailsEn = "Charge ATMCard By Parent Wallet";
            }

            /// <summary>
            /// TransactionInfo: Refund From ATMCard To Parent Wallet
            /// <para>TransactionDirection: - in ATMCardTransaction, + in ParentWalletTransaction</para>
            /// </summary>
            public static class RefundFromATMCardToParent
            {
                public const string TitleAr = "استرداد";
                public const string TitleEn = "Refund";
                public const string DetailsAr = "استرداد من كارت الإبن إلى محفظة الوالد";
                public const string DetailsEn = "Refund From ATMCard To Parent Wallet";
            }

            /// <summary>
            /// TransactionInfo: Pay By ATM Card To Vendor POS
            /// <para>TransactionDirection: - in ATMCardTransaction, + in POSMachineTransaction</para>
            /// </summary>
            public static class PayByATMCardToVendor 
            {
                public const string TitleAr = "دفع";
                public const string TitleEn = "Pay";
                public const string DetailsAr = "دفع بواسطة كارت الإبن للتاجر";
                public const string DetailsEn = "Pay By ATM Card To Vendor POS";
            }

            /// <summary>
            /// TransactionInfo: Refund From Vendor POS To ATMCard
            /// <para>TransactionDirection: + in ATMCardTransaction, - in POSMachineTransaction</para>
            /// </summary>
            public static class RefundFromVendorToATMCard 
            {
                public const string TitleAr = "استرداد";
                public const string TitleEn = "Refund";
                public const string DetailsAr = "استرداد من التاجر إلى بطاقة الإبن";
                public const string DetailsEn = "Refund From Vendor POS To ATMCard";
            }
        }

        public static class POSMachineTransactionTypes
        {
            /// <summary>
            /// TransactionInfo: Charge POS Machine By Admin
            /// <para>TransactionDirection: + in POSMachineTransaction</para>
            /// </summary>
            public static class ChargePOSMachineByAdmin
            {
                public const string TitleAr = "شحن رصيد";
                public const string TitleEn = "Charge";
                public const string DetailsAr = "شحن رصيد للتاجر بواسطة الأدمن";
                public const string DetailsEn = "Charge POS Machine By Admin";
            }

            /// <summary>
            /// TransactionInfo: Pay Dues From POS Machine By Admin
            /// <para>TransactionDirection: - in POSMachineTransaction</para>
            /// </summary>
            public static class PayDuesFromPOSMachineByAdmin
            {
                public const string TitleAr = "دفع";
                public const string TitleEn = "Pay";
                public const string DetailsAr = "دفع المستحقات على التاجر بواسطة الأدمن";
                public const string DetailsEn = "Pay Dues From POS Machine By Admin";
            }

            /// <summary>
            /// TransactionInfo: Add Vendor Commission when Charge Parent Wallet By Vendor POS Machine
            /// <para>TransactionDirection: + in POSMachineTransaction</para>
            /// </summary>
            public static class AddVendorCommissionBySystem
            {
                public const string TitleAr = "عمولة";
                public const string TitleEn = "Commission";
                public const string DetailsAr = "أضف عمولة التاجر بواسطة النظام عند شحن محفظة الوالد بواسطة التاجر";
                public const string DetailsEn = "Add Vendor Commission By System when Charge Parent Wallet By Vendor POS Machine";
            }
        }
    }
}
