using System;
using System.Collections.Generic;

namespace Puzzle.Masroofi.Core.Models
{
    public partial class ATMCardType
    {
        public ATMCardType()
        {
            ATMCards = new HashSet<ATMCard>();
        }
        public Guid ATMCardTypeId { get; set; }
        public string TypeNameAr { get; set; }
        public string TypeNameEn { get; set; }
        public string FrontImageUrl { get; set; }
        public string BackImageUrl { get; set; }
        public decimal Cost { get; set; }
        public DateTime CreationDate { get; set; }
        public Guid CreationUser { get; set; }
        public DateTime? ModificationDate { get; set; }
        public Guid? ModificationUser { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        public virtual ICollection<ATMCard> ATMCards { get; set; }
    }
}
