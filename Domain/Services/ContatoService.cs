using Domain.Interfaces;
using Entities.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Services
{
    public class ContatoService : IContatoService
    {
        private readonly IContatoRepository _contatoRepository;

        public ContatoService(IContatoRepository contatoRepository)
        {
            _contatoRepository = contatoRepository;
        }

        public async Task AddContatoAsync(Contato contato)
        {
            await _contatoRepository.Add(contato);
        }

        public async Task AddContatosAsync(List<Contato> contatos)
        {
            await _contatoRepository.AddRange(contatos);
        }

        public async Task UpdateContatoAsync(Contato contato)
        {
            await _contatoRepository.Update(contato);
        }

        public async Task DeleteContatoAsync(Contato contato)
        {
            await _contatoRepository.Delete(contato);
        }

        public async Task<Contato> GetContatoByIdAsync(int id)
        {
            return await _contatoRepository.GetEntityById(id);
        }

        public async Task<List<Contato>> GetAllContatosAsync()
        {
            return await _contatoRepository.GetList();
        }

        public async Task<(List<Contato> Items, int TotalItems)> GetPagedList(int pageNumber, int pageSize, string searchTerm, string searchProperty, string orderByProperty, bool isAscending = true)
        {
            return await _contatoRepository.GetPagedList(pageNumber, pageSize, searchTerm, searchProperty, orderByProperty, isAscending);
        }
    }
}
