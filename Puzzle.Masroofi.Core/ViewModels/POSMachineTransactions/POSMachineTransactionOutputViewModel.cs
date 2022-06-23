using Puzzle.Masroofi.Core.Enums;
using System;
using System.Collections.Generic;

namespace Puzzle.Masroofi.Core.ViewModels.POSMachineTransactions
{
    public class POSMachineTransactionOutputViewModel
    {
        public Guid POSMachineTransactionId { get; set; }
        public Guid VendorId { get; set; }
        public Guid POSMachineId { get; set; }
        public Guid? ParentId { get; set; }
        public Guid? SonId { get; set; }
        public string SonName { get; set; }
        public string SonNameAr { get; set; }
        public string SonNameEn { get; set; }
        public Guid? ATMCardId { get; set; }
        public Guid? AdminUserId { get; set; }
        public Guid? ReferenceTransactionId { get; set; }
        public long TransactionNumber { get; set; }
        public decimal Amount { get; set; }
        public decimal Refunds { get; set; }
        public decimal NetAmount { get; set; }
        public int PaymentType { get; set; }
        public string PaymentTypeText { get; set; }
        public string TitleAr { get; set; }
        public string TitleEn { get; set; }
        public string DetailsAr { get; set; }
        public string DetailsEn { get; set; }
        public string Note { get; set; }
        public DateTime CreationDate { get; set; }
        public bool IsSuccess { get; set; }

        public List<POSMachineRefundTransactionOutputViewModel> RefundTransactions { get; set; }
    }

    public class POSMachineRefundTransactionOutputViewModel
    {
        public Guid POSMachineTransactionId { get; set; }
        public Guid? ATMCardId { get; set; }
        public long TransactionNumber { get; set; }
        public decimal Amount { get; set; }
        public int PaymentType { get; set; }
        public string PaymentTypeText { get; set; }
        public string TitleAr { get; set; }
        public string TitleEn { get; set; }
        public string DetailsAr { get; set; }
        public string DetailsEn { get; set; }
        public string Note { get; set; }
        public DateTime CreationDate { get; set; }
        public bool IsSuccess { get; set; }
    }
}
