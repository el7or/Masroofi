using System;

namespace Puzzle.Masroofi.Core.ViewModels.ATMCardTypes
{
    public class ATMCardTypeInputViewModel
    {
        public Guid? ATMCardTypeId { get; set; }
        public string TypeNameAr { get; set; }
        public string TypeNameEn { get; set; }
        public decimal Cost { get; set; }
        public string FrontImageUrl { get; set; }
        public string BackImageUrl { get; set; }
        public PuzzleFileInfo NewFrontImage { get; set; }
        public PuzzleFileInfo NewBackImage { get; set; }
    }
}
