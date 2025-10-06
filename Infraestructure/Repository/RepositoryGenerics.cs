using Domain.Interfaces.Specifications;
using Domain.Specifications;
using Entities.Entities;
using Entities.Models;
using Infraestructure.Configuration;
using Infraestructure.Extensions;
using Infraestructure.Interfaces.Generics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Infraestructure.Repository
{

    public class RepositoryGenerics<T> : IGeneric<T> where T : class
    {
        protected readonly ContextBase _context;

        public RepositoryGenerics(ContextBase context)
        {
            _context = context;
        }

        public virtual async Task Add(T Objeto)
        {
            await _context.Set<T>().AddAsync(Objeto);
            await _context.SaveChangesAsync();
        }

        public virtual async Task AddRange(IEnumerable<T> objetos)
        {
            if (objetos is null) throw 
                new ArgumentNullException(nameof(objetos));

            await _context.Set<T>().AddRangeAsync(objetos);
            await _context.SaveChangesAsync();
        }

        public virtual async Task Delete(T objeto)
        {
            _context.Set<T>().Remove(objeto);
            await _context.SaveChangesAsync();
        }
        public virtual async Task<T> GetEntityById(int id)
        {
            IQueryable<T> query = _context.Set<T>();

            // Incluir propriedades de navegação
            var entityType = _context.Model.FindEntityType(typeof(T));
            var navigations = entityType.GetNavigations();
            foreach (var navigation in navigations)
            {
                query = query.Include(navigation.Name);

                // Incluir propriedades de navegação aninhadas
                var nestedNavigations = navigation.ForeignKey.DeclaringEntityType.GetNavigations();
                foreach (var nestedNavigation in nestedNavigations)
                {
                    query = query.Include($"{navigation.Name}.{nestedNavigation.Name}");
                }
            }

            // Obter o nome da propriedade da chave primária
            var keyProperty = entityType.FindPrimaryKey().Properties.FirstOrDefault();

            if (keyProperty == null)
                throw new InvalidOperationException("A entidade não possui uma chave primária definida.");

            var keyName = keyProperty.Name;

            // Construir a expressão para filtrar pelo id
            var parameter = Expression.Parameter(typeof(T), "e");
            var propertyAccess = Expression.Property(parameter, keyName);
            var idValue = Expression.Constant(id);
            var equalsExpression = Expression.Equal(propertyAccess, idValue);
            var lambda = Expression.Lambda<Func<T, bool>>(equalsExpression, parameter);

            return await query.FirstOrDefaultAsync(lambda);
        }
        public virtual async Task<List<T>> GetList()
        {
            IQueryable<T> query = _context.Set<T>();

            // Incluir propriedades de navegação
            var entityType = _context.Model.FindEntityType(typeof(T));
            var navigations = entityType.GetNavigations();
            foreach (var navigation in navigations)
            {
                query = query.Include(navigation.Name);

                // Incluir propriedades de navegação aninhadas
                var nestedNavigations = navigation.ForeignKey.DeclaringEntityType.GetNavigations();
                foreach (var nestedNavigation in nestedNavigations)
                {
                    query = query.Include($"{navigation.Name}.{nestedNavigation.Name}");
                }
            }

            return await query.ToListAsync();
        }
        public virtual async Task<(List<T> Objeto, int TotalItems)> GetPagedList(
            int pageNumber,
            int pageSize,
            string searchTerm,
            string searchProperty,
            string orderByProperty,
            bool isAscending = true)
        {
            IQueryable<T> query = _context.Set<T>();

            // Incluir propriedades de navegação
            var entityType = _context.Model.FindEntityType(typeof(T));
            var navigations = entityType.GetNavigations();
            foreach (var navigation in navigations)
            {
                query = query.Include(navigation.Name);

                // Incluir propriedades de navegação aninhadas
                var nestedNavigations = navigation.ForeignKey.DeclaringEntityType.GetNavigations();
                foreach (var nestedNavigation in nestedNavigations)
                {
                    query = query.Include($"{navigation.Name}.{nestedNavigation.Name}");
                }
            }

            // APLICAÇÃO DO FILTRO (corrigido: agora atribui o resultado do Where)
            if (!string.IsNullOrWhiteSpace(searchTerm) && !string.IsNullOrWhiteSpace(searchProperty))
            {
                var provider = _context.Database.ProviderName ?? string.Empty;
                var isPostgreSQL = provider.Contains("PostgreSQL", StringComparison.OrdinalIgnoreCase) ||
                                    provider.Contains("Npgsql", StringComparison.OrdinalIgnoreCase);

                if (isPostgreSQL)
                {
                    query = query.Where(e => EF.Functions.ILike(
                        EF.Property<string>(e, searchProperty),
                        $"%{searchTerm.Trim()}%"));
                }
                else
                {
                    query = query.Where(e => EF.Property<string>(e, searchProperty) != null &&
                                              EF.Functions.Like(EF.Property<string>(e, searchProperty)!, $"%{searchTerm.Trim()}%"));
                }
            }

            if (!string.IsNullOrEmpty(orderByProperty))
            {
                var parameter = Expression.Parameter(typeof(T), "e");
                var property = Expression.PropertyOrField(parameter, orderByProperty);
                var lambda = Expression.Lambda(property, parameter);

                string methodName = isAscending ? "OrderBy" : "OrderByDescending";
                var resultExpression = Expression.Call(
                    typeof(Queryable),
                    methodName,
                    new Type[] { typeof(T), property.Type },
                    query.Expression,
                    Expression.Quote(lambda));

                query = query.Provider.CreateQuery<T>(resultExpression);
            }

            var totalItems = await query.CountAsync();
            var objetos = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (objetos, totalItems);
        }

        public virtual async Task<(List<T> Itens, int TotalItems)>
            GetPagedListConditions(PesquisaSimples pesquisa)
        {
            IQueryable<T> query = _context.Set<T>();

            // 1) includes
            var entityType = _context.Model.FindEntityType(typeof(T));
            foreach (var nav in entityType.GetNavigations())
            {
                query = query.Include(nav.Name);
                foreach (var nn in nav.ForeignKey.DeclaringEntityType.GetNavigations())
                    query = query.Include($"{nav.Name}.{nn.Name}");
            }

            // 2) monta a spec composta
            var filtros = pesquisa.Filtros
                .Where(f => !string.IsNullOrWhiteSpace(f.Campo)
                         && !string.IsNullOrWhiteSpace(f.Valor))
                .ToList();

            if (filtros.Any())
            {
                // começa pela primeira FilterSpecification
                ISpecification<T> spec = new FilterSpecification<T>(filtros[0]);

                // reduz os demais
                for (int i = 1; i < filtros.Count; i++)
                {
                    var filtro = filtros[i];
                    var nextSpec = new FilterSpecification<T>(filtro);

                    switch (filtro.LogicalOperator)
                    {
                        case "Or":
                            spec = new OrSpecification<T>(spec, nextSpec);
                            break;

                        case "Not":
                            // !nextSpec  e combinado como AND: spec AND (NOT nextSpec)
                            spec = new AndSpecification<T>(
                                spec,
                                new NotSpecification<T>(nextSpec)
                            );
                            break;

                        default: // "And"
                            spec = new AndSpecification<T>(spec, nextSpec);
                            break;
                    }
                }

                // aplica de uma vez só
                query = query.ApplySpecification(spec);
            }

            // 3) ordenação
            if (!string.IsNullOrWhiteSpace(pesquisa.OrderByProperty))
            {
                query = pesquisa.IsAscending
                    ? query.OrderBy(e => EF.Property<object>(e, pesquisa.OrderByProperty))
                    : query.OrderByDescending(e => EF.Property<object>(e, pesquisa.OrderByProperty));
            }

            // 4) conta
            var total = await query.CountAsync();

            // 5) pagina
            var itens = await query
                .Skip((pesquisa.PageNumber - 1) * pesquisa.PageSize)
                .Take(pesquisa.PageSize)
                .ToListAsync();

            return (itens, total);
        }

        public virtual async Task Update(T objeto)
        {
            _context.Set<T>().Update(objeto);
            await _context.SaveChangesAsync();
        }
    }
}
