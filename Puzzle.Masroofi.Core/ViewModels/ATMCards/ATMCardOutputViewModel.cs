using Puzzle.Masroofi.Core.ViewModels.ATMCardTypes;
using Puzzle.Masroofi.Core.ViewModels.Parents;
using Puzzle.Masroofi.Core.ViewModels.Sons;
using System;
using System.Collections.Generic;

namespace Puzzle.Masroofi.Core.ViewModels.ATMCards
{
    public class ATMCardOutputViewModel
    {
        public Guid ATMCardId { get; set; }
        public Guid SonId { get; set; }
        public Guid ATMCardTypeId { get; set; }
        public long CardId { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public string ShortNumber { get; set; }
        public string CardNumber { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string ExpiryMonth { get; set; }
        public string ExpiryYear { get; set; }
        public string SecurityCode { get; set; }
        public int Status { get; set; }
        public string StatusText { get; set; }
        public string RejectedReason { get; set; }
        public DateTime CreationDate { get; set; }
        public bool IsCurrentATMCard { get; set; }      

        public ATMCardTypeOutputViewModel ATMCardType { get; set; }
    }
}
