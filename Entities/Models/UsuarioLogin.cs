using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    public class UsuarioRegister
    {
        public string Login { get; set; }
        public string Senha { get; set; }
    }
    public class UsuarioUpdateModel
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
    }
}
