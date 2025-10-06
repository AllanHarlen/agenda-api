using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Enums
{
    public enum RoleEnum
    {
        [Display(Name = "Administrador")]
        Administrador = 1,

        [Display(Name = "Servidor")]
        Servidor = 2,

        [Display(Name = "Operador")]
        Operador = 3,

        [Display(Name = "Técnico de Campo")]
        TecnicoDeCampo = 4
    }
}
