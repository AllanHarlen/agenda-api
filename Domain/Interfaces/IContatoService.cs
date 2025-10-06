using Entities.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Services
{
    public interface IContatoService
    {
        Task AddContatoAsync(Contato contato);
        Task AddContatosAsync(List<Contato> contatos);
        Task UpdateContatoAsync(Contato contato);
        Task DeleteContatoAsync(Contato contato);
        Task<Contato> GetContatoByIdAsync(int id);
        Task<List<Contato>> GetAllContatosAsync();
        Task<(List<Contato> Items, int TotalItems)> GetPagedList(
            int pageNumber,
            int pageSize,
            string searchTerm,
            string searchProperty,
            string orderByProperty,
            bool isAscending = true);
    }
}
