using System;
using System.ComponentModel.DataAnnotations;

namespace Puzzle.Masroofi.Core.ViewModels.ATMCards
{
    public class ATMCardInputViewModel
    {
        public Guid? ATMCardId { get; set; }
        public Guid ATMCardTypeId { get; set; }
        public Guid SonId { get; set; }

        public int Status { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public string ShortNumber { get; set; }
        public string CardNumber { get; set; }
        public string SecurityCode { get; set; }

        [DataType(DataType.Date)]
        public DateTime? ExpiryDate { get; set; }
    }
}
