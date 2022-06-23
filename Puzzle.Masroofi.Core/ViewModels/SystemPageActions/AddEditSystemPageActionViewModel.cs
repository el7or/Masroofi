using System;

namespace Puzzle.Masroofi.Core.ViewModels.SystemPageActions
{
    public class AddEditSystemPageActionViewModel
    {
        public Guid SystemPageActionId { get; set; }
        public string ActionArabicName { get; set; }
        public string ActionEnglishName { get; set; }
        public string ActionUniqueName { get; set; }
        public Guid SystemPageId { get; set; }
    }
}
