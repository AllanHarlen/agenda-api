using System.Collections.Generic;
using System.Threading.Tasks;
using Entities.Models;
using Entities.Entities;

namespace Domain.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<IEnumerable<RoleDto>> GetRolesAsync();
        Task<(List<Usuario> Itens, int TotalItems)> GetPagedListConditions(PesquisaSimples pesquisaSimples);
        // Registra um novo usuário e, opcionalmente, atribui roles
        Task<(bool Success, string? ErrorMessage, Usuario? Usuario)> RegisterAsync(Usuario usuario, string senha, IEnumerable<string>? roles = null);
    }
}
