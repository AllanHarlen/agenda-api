using Entities.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Services
{
    public interface IAgendamentoService
    {
        Task AddAgendamentoAsync(Agendamento agendamento);
        Task AddAgendamentosAsync(List<Agendamento> agendamentos);
        Task UpdateAgendamentoAsync(Agendamento agendamento);
        Task DeleteAgendamentoAsync(Agendamento agendamento);
        Task<Agendamento> GetAgendamentoByIdAsync(int id);
        Task<List<Agendamento>> GetAllAgendamentosAsync();
        Task<(List<Agendamento> Items, int TotalItems)> GetPagedList(
            int pageNumber,
            int pageSize,
            string searchTerm,
            string searchProperty,
            string orderByProperty,
            bool isAscending = true);
    }
}
