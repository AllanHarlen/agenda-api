using System;

namespace Entities.Models
{
    public class AgendamentoCalendarModel
    {
        public int Id { get; set; }
        public int ContatoId { get; set; }
        public string? ContatoNome { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string Time { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}