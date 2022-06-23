using System;

namespace Puzzle.Masroofi.Core.ViewModels.ATMCards
{
    public class LostATMCardViewModel
    {
        public Guid CurrentATMCardId { get; set; }
        public ATMCardInputViewModel NewATMCard { get; set; }
    }
}
