using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Entities
{
    [Table("agendamento")]
    public class Agendamento
    {
        [Key]
        [Column("codg")]
        public int Codg { get; set; }

        [Required]
        [Column("data_hora")]
        public DateTime DataHora { get; set; }

        [MaxLength(200)]
        [Column("dscr")]
        public string? Dscr { get; set; }

        [Required]
        [Column("contato_id")]
        public int ContatoId { get; set; }

        public Contato Contato { get; set; }
    }
}
