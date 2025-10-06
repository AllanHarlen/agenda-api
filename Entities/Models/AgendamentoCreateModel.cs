using System.ComponentModel.DataAnnotations;

namespace Entities.Models
{
    public class AgendamentoCreateModel
    {
        [Required]
        public int ContatoId { get; set; }

        [Required]
        public DateTime DataHora { get; set; }

        [MaxLength(200)]
        public string? Dscr { get; set; }
    }
}
