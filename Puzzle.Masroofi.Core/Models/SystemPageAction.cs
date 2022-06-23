using System;
using System.Collections.Generic;

namespace Puzzle.Masroofi.Core.Models
{
    public partial class SystemPageAction
    {
        public SystemPageAction()
        {
            ActionsInRoles = new HashSet<ActionsInRoles>();
        }
        public Guid SystemPageActionId { get; set; }
        public string ActionArabicName { get; set; }
        public string ActionEnglishName { get; set; }
        public string ActionUniqueName { get; set; }
        public Guid SystemPageId { get; set; }

        public virtual SystemPage SystemPage { get; set; }

        public virtual ICollection<ActionsInRoles> ActionsInRoles { get; set; }
    }
}
