using Domain.Services;
using Entities.Entities;
using Entities.Models;
using Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Entities.Extensions;
using WebApis.Token;
using Entities.Enums;

namespace WebApis.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly UserManager<Usuario> _userManager;
        private readonly SignInManager<Usuario> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUsuarioRepository _userRepository;

        public UsuarioController(
            UserManager<Usuario> userManager,
            SignInManager<Usuario> signInManager,
            RoleManager<IdentityRole> roleManager,
            IUsuarioRepository userRepository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _userRepository = userRepository;
        }

        [AllowAnonymous]
        [HttpGet("GruposUsuarios")]
        public async Task<IActionResult> GruposUsuarios()
        {
            var roles = await _userRepository.GetRolesAsync();
            return Ok(roles);
        }

        [HttpGet("UsuariosPorGrupo/{roleName}")]
        public async Task<IActionResult> UsuariosPorGrupo(string roleName)
        {
            var users = await _userManager.GetUsersInRoleAsync(roleName);
            return Ok(users);
        }

        [AllowAnonymous]
        [HttpPost("AutorizarUsuario")]
        public async Task<IActionResult> AutorizarUsuario([FromBody] UsuarioLogin login)
        {
            if (string.IsNullOrWhiteSpace(login.Login) || string.IsNullOrEmpty(login.Senha))
                return Unauthorized("Vazio");

            // Se for ADM, retorna token pré-configurado
            if (login.Login == "ADM" && login.Senha == "ADM")
            {
                string[] cargos = { "Administrador" };
                Usuario usuario = new Usuario
                {
                    Id = Guid.NewGuid().ToString(),
                    UserName = "ADM",
                    Status = StatusUsuario.Ativo,
                };

                Settings config = new Settings();
                var tokenjwt = config.GenerateToken(usuario, cargos);
                var usuarioResponses = new UsuarioResponse
                {
                    Auth = tokenjwt,
                    Usuario = new UsuarioInfo
                    {
                        Id = usuario.Id,
                        Login = usuario.UserName,
                        Roles = cargos
                    }
                };
                return Ok(usuarioResponses);
            }

            // Recupera o usuário pelo login informado
            var user = await _userManager.FindByNameAsync(login.Login);
            if (user == null)
                return Unauthorized("Usuário ou senha inválidos.");

            // Verifica a senha
            var passwordValid = await _userManager.CheckPasswordAsync(user, login.Senha);
            if (!passwordValid)
                return Unauthorized("Usuário ou senha inválidos.");

            if (user.Status == StatusUsuario.Inativo || user.Status == StatusUsuario.PreCadastro)
                return Unauthorized("Seu login está inativo ou pré-cadastramento, entre em contato com administrador para ativa-lo.");

            // Coleta as roles do usuário (caso seja necessário para o token)
            var roles = await _userManager.GetRolesAsync(user);
            var lstRoles = roles.ToList();

            Settings configuracoes = new Settings();
            var token = configuracoes.GenerateToken(user, roles);

            // Atualiza a resposta do usuário para incluir as roles
            var usuarioResponse = new UsuarioResponse
            {
                Auth = token,
                Usuario = new UsuarioInfo
                {
                    Id = user.Id,
                    Login = user.UserName,
                    Roles = lstRoles
                }
            };

            return Ok(usuarioResponse);
        }

        [AllowAnonymous]
        [HttpPost("RegistrarUsuario")]
        public async Task<IActionResult> RegistrarUsuario([FromBody] UsuarioRegister register)
        {
            if (register == null || string.IsNullOrWhiteSpace(register.Login) || string.IsNullOrWhiteSpace(register.Senha))
                return BadRequest("Login e Senha são obrigatórios.");

            var novoUsuario = new Usuario
            {
                UserName = register.Login,
                Status = StatusUsuario.Ativo
            };

            var result = await _userRepository.RegisterAsync(novoUsuario, register.Senha);
            if (!result.Success)
            {
                if (!string.IsNullOrWhiteSpace(result.ErrorMessage) && result.ErrorMessage.Contains("existe", StringComparison.OrdinalIgnoreCase))
                    return Conflict(result.ErrorMessage);
                return BadRequest(result.ErrorMessage);
            }

            return CreatedAtAction(nameof(GetUsuario), new { id = result.Usuario!.Id }, new { result.Usuario.Id, Login = result.Usuario.UserName });
        }

        [HttpGet("ListaUsuarios")]
        public async Task<ActionResult<IEnumerable<Usuario>>> GetUsuarios()
        {
            var usuarios = _userManager.Users.ToList();
            return Ok(usuarios);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Usuario>> GetUsuario(string id)
        {
            var usuario = await _userManager.FindByIdAsync(id);
            if (usuario == null)
                return NotFound();

            return Ok(usuario);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUsuario(string id, [FromBody] UsuarioInfoBasic model)
        {
            if (id != model.Id)
                return BadRequest("IDs não correspondem.");

            var usuario = await _userManager.FindByIdAsync(id);
            if (usuario == null)
                return NotFound();

            // Atualiza dados básicos
            usuario.Email = model.Email;
            usuario.UserName = model.UserName;
            usuario.PhoneNumber = model.Phone;

            if (usuario.Status == StatusUsuario.Inativo || usuario.Status == StatusUsuario.PreCadastro)
                usuario.Status = StatusUsuario.Ativo;

            var result = await _userManager.UpdateAsync(usuario);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            // Atualiza as roles
            var desiredRoles = model.Roles?.ToArray() ?? Array.Empty<string>();
            var currentRoles = await _userManager.GetRolesAsync(usuario);

            // Remove roles que não estão no array desejado
            var rolesToRemove = currentRoles.Where(r => !desiredRoles.Contains(r)).ToList();
            if (rolesToRemove.Any())
            {
                var removeResult = await _userManager.RemoveFromRolesAsync(usuario, rolesToRemove);
                if (!removeResult.Succeeded)
                    return BadRequest(removeResult.Errors);
            }

            // Adiciona roles do array desejado que o usuário ainda não possui
            foreach (var roleId in desiredRoles)
            {
                // Busca o role pelo ID
                var existRole = await _roleManager.FindByIdAsync(roleId);
                if (existRole == null)
                    return BadRequest($"O role {roleId} não existe.");

                if (!currentRoles.Contains(existRole.Name))
                {
                    var addResult = await _userManager.AddToRoleAsync(usuario, existRole.Name);
                    if (!addResult.Succeeded)
                        return BadRequest(addResult.Errors);
                }
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsuario(string id)
        {
            var usuario = await _userManager.FindByIdAsync(id);
            if (usuario == null)
                return NotFound();

            var result = await _userManager.DeleteAsync(usuario);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return NoContent();
        }

        [HttpPost("ListarUsuariosPaginados")]
        public async Task<IActionResult> GetPagedUsuarios([FromBody] PesquisaSimples pesquisaSimples)
        {
            // Utiliza o UserRepository para paginação avançada com displayName nas roles
            (List<Usuario> usuarios, int totalItems) = await _userRepository.GetPagedListConditions(pesquisaSimples);

            var totalPages = (int)Math.Ceiling(totalItems / (double)pesquisaSimples.PageSize);
            var usuariosInfoBasic = new List<UsuarioInfoBasic>();
            foreach (var usuario in usuarios)
            {
                usuariosInfoBasic.Add(new UsuarioInfoBasic
                {
                    Id = usuario.Id,
                    UserName = usuario.UserName,
                    Email = usuario.Email,
                    Phone = usuario.PhoneNumber,
                    Roles = usuario.Roles
                });
            }

            return Ok(new
            {
                Usuarios = usuariosInfoBasic,
                TotalItems = totalItems,
                pesquisaSimples.PageNumber,
                pesquisaSimples.PageSize,
                TotalPages = totalPages
            });
        }

    }
}
