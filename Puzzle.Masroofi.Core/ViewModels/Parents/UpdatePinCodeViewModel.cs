using System;

namespace Puzzle.Masroofi.Core.ViewModels.Parents
{
    public class UpdatePinCodeViewModel
    {
        public Guid ParentId { get; set; }
        public string OldPinCode { get; set; }
        public string PinCode { get; set; }
    }
}
