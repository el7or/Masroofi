using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puzzle.Masroofi.Core.Models
{
    public partial class ParentMobileRegistration
    {
        public Guid ParentMobileRegistrationId { get; set; }
        public Guid ParentId { get; set; }
        public string RegisterId { get; set; }
        public string RegisterType { get; set; }
        public DateTime CreationDate { get; set; }
        public Guid CreationUser { get; set; }
        public DateTime? ModificationDate { get; set; }
        public Guid? ModificationUser { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        public virtual Parent Parent { get; set; }
    }
}
