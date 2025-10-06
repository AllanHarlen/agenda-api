namespace Entities.Models
{
    public class PesquisaSimples
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string OrderByProperty { get; set; }
        public bool IsAscending { get; set; } = true;
        public List<PesquisaAvancada>? Filtros { get; set; } = new();
    }
}
