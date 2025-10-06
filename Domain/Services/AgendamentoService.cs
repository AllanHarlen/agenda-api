using Domain.Interfaces;
using Entities.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Services
{
    public class AgendamentoService : IAgendamentoService
    {
        private readonly IAgendamentoRepository _agendamentoRepository;

        public AgendamentoService(IAgendamentoRepository agendamentoRepository)
        {
            _agendamentoRepository = agendamentoRepository;
        }

        public async Task AddAgendamentoAsync(Agendamento agendamento)
        {
            await _agendamentoRepository.Add(agendamento);
        }

        public async Task AddAgendamentosAsync(List<Agendamento> agendamentos)
        {
            await _agendamentoRepository.AddRange(agendamentos);
        }

        public async Task UpdateAgendamentoAsync(Agendamento agendamento)
        {
            await _agendamentoRepository.Update(agendamento);
        }

        public async Task DeleteAgendamentoAsync(Agendamento agendamento)
        {
            await _agendamentoRepository.Delete(agendamento);
        }

        public async Task<Agendamento> GetAgendamentoByIdAsync(int id)
        {
            return await _agendamentoRepository.GetEntityById(id);
        }

        public async Task<List<Agendamento>> GetAllAgendamentosAsync()
        {
            return await _agendamentoRepository.GetList();
        }

        public async Task<(List<Agendamento> Items, int TotalItems)> GetPagedList(int pageNumber, int pageSize, string searchTerm, string searchProperty, string orderByProperty, bool isAscending = true)
        {
            return await _agendamentoRepository.GetPagedList(pageNumber, pageSize, searchTerm, searchProperty, orderByProperty, isAscending);
        }
    }
}
