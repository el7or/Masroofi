using Puzzle.Masroofi.Core.Enums;
using System;

namespace Puzzle.Masroofi.Core.ViewModels.ATMCardTransactions
{
    public class ATMCardTransactionOutputViewModel
    {
        public Guid ATMCardTransactionId { get; set; }
        public Guid ATMCardId { get; set; }
        public Guid SonId { get; set; }
        public Guid? ParentId { get; set; }
        public Guid? VendorId { get; set; }
        public Guid? POSMachineId { get; set; }
        public long TransactionNumber { get; set; }
        public decimal Amount { get; set; }
        public int PaymentType { get; set; }
        public string PaymentTypeText { get; set; }
        public string TitleAr { get; set; }
        public string TitleEn { get; set; }
        public string DetailsAr { get; set; }
        public string DetailsEn { get; set; }
        public DateTime CreationDate { get; set; }
        public bool IsSuccess { get; set; }
    }
}
