using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Entities;
using Entities.Models;

namespace Infraestructure.Interfaces.Generics
{
    public interface IGeneric<T> where T : class
    {
        Task Add(T Objeto);
        Task AddRange(IEnumerable<T> Objetos);
        Task Update(T Objeto);
        Task Delete(T Objeto);
        Task<T> GetEntityById(int Id);
        Task<List<T>> GetList();
        Task<(List<T> Objeto, int TotalItems)> GetPagedList(
                    int pageNumber,
                    int pageSize,
                    string searchTerm,
                    string searchProperty,
                    string orderByProperty,
                    bool isAscending = true);
        Task<(List<T> Itens, int TotalItems)> GetPagedListConditions(PesquisaSimples pesquisaSimples);
    }
}
