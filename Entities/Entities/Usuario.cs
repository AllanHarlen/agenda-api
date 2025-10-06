using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Entities
{
    public enum StatusUsuario
    {
        Ativo,
        Inativo,
        PreCadastro
    }
    public class Usuario : IdentityUser
    {
        public StatusUsuario Status { get; set; }
        [NotMapped]
        public List<string> Roles { get; set; } = new List<string>();
    }
}
