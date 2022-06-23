using System;

namespace Puzzle.Masroofi.Core.ViewModels.Parents
{
    public class ParentAuthenticateViewModel
    {
        public object Token { get; set; }
        public Guid ParentId { get; set; }
        public ParentOutputViewModel ParentInfo { get; set; }
        public string CurrencySymbol { get; set; }
        public string IsValidateSonFace { get; set; }
    }
}
