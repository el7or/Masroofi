using System;

namespace Puzzle.Masroofi.Core.ViewModels.ATMCards
{
    public class ReplacedATMCardViewModel
    {
        public Guid CurrentATMCardId { get; set; }
        public ATMCardInputViewModel NewATMCard { get; set; }
    }
}
