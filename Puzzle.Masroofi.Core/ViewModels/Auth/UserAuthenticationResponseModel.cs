using System;
using System.Collections.Generic;

namespace Puzzle.Masroofi.Core.ViewModels.Auth
{
    public class UserAuthenticationResponseModel
    {
        public List<Guid> RolesIds { get; set; }
        public List<string> UserActions { get; set; }
    }
}
