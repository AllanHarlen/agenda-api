using Domain.Services;
using Entities.Entities;
using Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApis.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ContatoController : ControllerBase
    {
        private readonly IContatoService _contatoService;

        public ContatoController(IContatoService contatoService)
        {
            _contatoService = contatoService;
        }

        [HttpGet("ListarContatosPaginados")]
        public async Task<ActionResult> GetAllContatosPaginados(
            int pageNumber = 1,
            int pageSize = 10,
            string? searchTerm = null,
            string searchProperty = "Nome",
            string orderByProperty = "Codg",
            bool isAscending = true)
        {
            var (contatos, totalItems) = await _contatoService.GetPagedList(
                pageNumber,
                pageSize,
                searchTerm ?? string.Empty,
                searchProperty,
                orderByProperty,
                isAscending);

            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            var response = new
            {
                Contatos = contatos,
                TotalItems = totalItems,
                TotalPages = totalPages,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            return Ok(response);
        }

        [HttpGet("ListarTodosContatos")]
        public async Task<ActionResult<List<Contato>>> GetAllContatos()
        {
            var contatos = await _contatoService.GetAllContatosAsync();
            return Ok(contatos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Contato>> GetContatoById(int id)
        {
            var contato = await _contatoService.GetContatoByIdAsync(id);
            if (contato == null)
                return NotFound();
            return Ok(contato);
        }

        [HttpPost]
        public async Task<ActionResult> AddContato([FromBody] ContatoModel model)
        {
            if (model == null)
                return BadRequest();

            var entity = new Contato
            {
                Codg = 0,
                Nome = model.Nome,
                Email = model.Email,
                Telefone = model.Telefone,
                Agendamentos = model.Agendamentos?.Select(a => new Agendamento
                {
                    Codg = 0,
                    DataHora = a.DataHora,
                    Dscr = a.Dscr
                }).ToList() ?? new List<Agendamento>()
            };

            await _contatoService.AddContatoAsync(entity);
            return CreatedAtAction(nameof(GetContatoById), new { id = entity.Codg }, entity);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateContato(int id, [FromBody] ContatoModel model)
        {
            if (model == null) return BadRequest();

            var existing = await _contatoService.GetContatoByIdAsync(id);
            if (existing == null) return NotFound();

            existing.Nome = model.Nome;
            existing.Email = model.Email;
            existing.Telefone = model.Telefone;
            // Agendamentos não atualizados aqui por ausência de IDs nos itens do model

            await _contatoService.UpdateContatoAsync(existing);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteContato(int id)
        {
            var contato = await _contatoService.GetContatoByIdAsync(id);
            if (contato == null)
                return NotFound();

            await _contatoService.DeleteContatoAsync(contato);
            return NoContent();
        }
    }
}
