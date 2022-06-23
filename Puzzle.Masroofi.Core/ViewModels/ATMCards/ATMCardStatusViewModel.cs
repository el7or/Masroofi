using Puzzle.Masroofi.Core.Enums;
using System;

namespace Puzzle.Masroofi.Core.ViewModels.ATMCards
{
    public class ATMCardStatusViewModel
    {
        public Guid ATMCardId { get; set; }
        public Guid SonId { get; set; }
        public ATMCardStatus Status { get; set; }
        public string RejectedReason { get; set; }
        public string Password { get; set; }
    }
}
