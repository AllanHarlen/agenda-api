using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Entities.Models
{
    public class AgendamentoModel
    {
        [Required]
        public DateTime DataHora { get; set; }
        public string? Dscr { get; set; }
    }

    public class ContatoModel
    {
        [Required]
        [MaxLength(100)]
        public string Nome { get; set; }
        [MaxLength(200)]
        public string? Email { get; set; }
        [MaxLength(20)]
        public string? Telefone { get; set; }
        public List<AgendamentoModel>? Agendamentos { get; set; }
    }
}
