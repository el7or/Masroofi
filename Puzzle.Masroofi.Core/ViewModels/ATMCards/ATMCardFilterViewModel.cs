using Puzzle.Masroofi.Core.Enums;
using System;

namespace Puzzle.Masroofi.Core.ViewModels.ATMCards
{
    public class ATMCardFilterViewModel : PagedInput
    {
        public Guid? SonId { get; set; }
        public Guid? ATMCardTypeId { get; set; }
        public ATMCardStatus? Status { get; set; }
        public string Name { get; set; }

        public string ParentName { get; set; }

        public string SonName { get; set; }
    }
}
