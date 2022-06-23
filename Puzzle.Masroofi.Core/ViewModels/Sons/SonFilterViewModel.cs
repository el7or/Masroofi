using Puzzle.Masroofi.Core.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace Puzzle.Masroofi.Core.ViewModels.Sons
{
    public class SonFilterViewModel : PagedInput
    {
        public Guid? ParentId { get; set; }
        public string SonName { get; set; }
        public Gender? Gender { get; set; }

        [DataType(DataType.Date)]
        public DateTime? Birthdate { get; set; }
    }
}
