using System;

namespace Puzzle.Masroofi.Core.ViewModels.ATMCardTypes
{
    public class ATMCardTypeOutputViewModel
    {
        public Guid ATMCardTypeId { get; set; }
        public string TypeNameAr { get; set; }
        public string TypeNameEn { get; set; }

        public string TypeName { get; set; }
        public string FrontImageUrl { get; set; }
        public string BackImageUrl { get; set; }
        public decimal Cost { get; set; }
        public DateTime CreationDate { get; set; }
        public bool IsActive { get; set; }
    }
}
