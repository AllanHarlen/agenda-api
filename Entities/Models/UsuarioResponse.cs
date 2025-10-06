using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace Entities.Models
{
    public class UsuarioResponse
    {
        public string Auth { get; set; }
        public UsuarioInfo Usuario { get; set; }
    }

    public class UsuarioInfo
    {
        public string Id { get; set; }
        public string Login { get; set; }
        public IEnumerable<string> Roles { get; set; }
    }

    public class UsuarioInfoBasic
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public IEnumerable<string> Roles { get; set; }
    }
}
