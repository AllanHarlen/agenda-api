namespace Entities.Models
{
    public class PesquisaAvancada
    {
        public string Campo { get; set; }
        public string Valor { get; set; }
        public string Condicional { get; set; } = "==";
        public string LogicalOperator { get; set; } = "And";
    }
}
