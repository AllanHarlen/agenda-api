using Entities.Models;
using Entities.Extensions;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infraestructure.Configuration;
using Microsoft.EntityFrameworkCore;
using Entities.Entities;
using Infraestructure.Repository;
using Entities.Enums;
using Domain.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Infraestructure.Repository
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly ContextBase _db;
        private readonly UserManager<Usuario> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        // Construtor padrão
        public UsuarioRepository(ContextBase db)
        {
            _db = db;
        }

        // Construtor com DI para Identity managers
        public UsuarioRepository(ContextBase db, UserManager<Usuario> userManager, RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // Registra um novo usuário no Identity e, opcionalmente, atribui roles
        public async Task<(bool Success, string? ErrorMessage, Usuario? Usuario)> RegisterAsync(Usuario usuario, string senha, IEnumerable<string>? roles = null)
        {
            // Verifica duplicidade por UserName
            var existing = await _userManager.FindByNameAsync(usuario.UserName);
            if (existing != null)
                return (false, "Usuário já existe.", null);

            // Define status padrão caso não informado corretamente
            if (!Enum.IsDefined(typeof(StatusUsuario), usuario.Status))
                usuario.Status = StatusUsuario.Ativo;

            var createResult = await _userManager.CreateAsync(usuario, senha);
            if (!createResult.Succeeded)
            {
                var error = string.Join("; ", createResult.Errors.Select(e => e.Description));
                return (false, error, null);
            }

            // Atribui roles (aceita Id ou Nome)
            if (roles != null && roles.Any())
            {
                var roleNames = new List<string>();
                foreach (var role in roles)
                {
                    var byId = await _roleManager.FindByIdAsync(role);
                    if (byId != null)
                    {
                        roleNames.Add(byId.Name);
                        continue;
                    }
                    var byName = await _roleManager.FindByNameAsync(role);
                    if (byName != null)
                    {
                        roleNames.Add(byName.Name);
                    }
                }

                if (roleNames.Any())
                {
                    var roleResult = await _userManager.AddToRolesAsync(usuario, roleNames.Distinct());
                    if (!roleResult.Succeeded)
                    {
                        var error = string.Join("; ", roleResult.Errors.Select(e => e.Description));
                        return (false, error, null);
                    }
                }
            }

            return (true, null, usuario);
        }

        public async Task<IEnumerable<RoleDto>> GetRolesAsync()
        {
            var roles = await _db.Roles
                .Join(_db.Roles, ur => ur.Id, r => r.Id, (ur, r) => new { r.Id, r.Name })
                .ToListAsync();

            return roles.Select(role =>
            {
                string displayName = role.Name;
                if (Enum.TryParse(typeof(RoleEnum), role.Name, out var enumValue))
                {
                    displayName = ((RoleEnum)enumValue).GetDisplayName();
                }
                return new RoleDto { Id = role.Id, Name = role.Name, DisplayName = displayName };
            });
        }

        public async Task<(List<Usuario> Itens, int TotalItems)> GetPagedListConditions(PesquisaSimples pesquisaSimples)
        {
            IQueryable<Usuario> query = _db.Usuarios.AsQueryable();

            // 1) includes (se necessário, pode incluir relacionamentos)
            // Exemplo: query = query.Include(u => u.Historicos);

            // 2) filtros
            var filtros = pesquisaSimples.Filtros?.Where(f => !string.IsNullOrWhiteSpace(f.Campo) && !string.IsNullOrWhiteSpace(f.Valor)).ToList();
            if (filtros != null && filtros.Any())
            {
                foreach (var filtro in filtros)
                {
                    var propInfo = typeof(Usuario).GetProperty(filtro.Campo);
                    if (propInfo != null)
                    {
                        if (propInfo.PropertyType == typeof(string))
                        {
                            query = query.Where(u => EF.Property<string>(u, filtro.Campo).Contains(filtro.Valor));
                        }
                        else if (propInfo.PropertyType.IsEnum)
                        {
                            if (Enum.TryParse(propInfo.PropertyType, filtro.Valor, out var enumValue))
                            {
                                query = query.Where(u => EF.Property<object>(u, filtro.Campo).Equals(enumValue));
                            }
                        }
                        else
                        {
                            // Para outros tipos, tenta conversão direta
                            var convertedValue = Convert.ChangeType(filtro.Valor, propInfo.PropertyType);
                            query = query.Where(u => EF.Property<object>(u, filtro.Campo).Equals(convertedValue));
                        }
                    }
                }
            }

            // 3) ordenação
            if (!string.IsNullOrWhiteSpace(pesquisaSimples.OrderByProperty))
            {
                query = pesquisaSimples.IsAscending
                    ? query.OrderBy(u => EF.Property<object>(u, pesquisaSimples.OrderByProperty))
                    : query.OrderByDescending(u => EF.Property<object>(u, pesquisaSimples.OrderByProperty));
            }

            // 4) conta
            var total = await query.CountAsync();

            // 5) pagina
            var itens = await query
                .Skip((pesquisaSimples.PageNumber - 1) * pesquisaSimples.PageSize)
                .Take(pesquisaSimples.PageSize)
                .ToListAsync();

            // 6) popula as roles (DisplayName)
            foreach (var usuario in itens)
            {
                var userRoles = await _db.UserRoles
                    .Where(ur => ur.UserId == usuario.Id)
                    .Join(_db.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => r.Name)
                    .ToListAsync();

                var displayNames = userRoles.Select(roleName =>
                {
                    if (Enum.TryParse(typeof(RoleEnum), roleName, out var enumValue))
                        return ((RoleEnum)enumValue).GetDisplayName();
                    return roleName;
                }).ToList();

                usuario.Roles = displayNames;
            }

            return (itens, total);
        }
    }
}
