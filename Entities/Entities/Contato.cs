using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace Entities.Entities
{
    [Table("contato")]
    public class Contato
    {
        [Key]
        [Column("codg")]
        public int Codg { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("nome")]
        public string Nome { get; set; }

        [MaxLength(200)]
        [Column("email")]
        public string? Email { get; set; }

        [MaxLength(20)]
        [Column("telefone")]
        public string? Telefone { get; set; }

        // Relacionamento 1:N (um Contato pode ter vários Agendamentos)
        public List<Agendamento> Agendamentos { get; set; } = new();
    }
}
