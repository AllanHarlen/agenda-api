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
    public class AgendamentoController : ControllerBase
    {
        private readonly IAgendamentoService _agendamentoService;

        public AgendamentoController(IAgendamentoService agendamentoService)
        {
            _agendamentoService = agendamentoService;
        }

        [HttpGet("ListarAgendamentosPaginados")]
        public async Task<ActionResult> GetAllAgendamentosPaginados(
            int pageNumber = 1,
            int pageSize = 10,
            string? searchTerm = null,
            string searchProperty = "Dscr",
            string orderByProperty = "Codg",
            bool isAscending = true)
        {
            var (items, totalItems) = await _agendamentoService.GetPagedList(
                pageNumber,
                pageSize,
                searchTerm ?? string.Empty,
                searchProperty,
                orderByProperty,
                isAscending);

            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            return Ok(new
            {
                Agendamentos = items,
                TotalItems = totalItems,
                TotalPages = totalPages,
                PageNumber = pageNumber,
                PageSize = pageSize
            });
        }

        [HttpGet("Calendario")]
        public async Task<ActionResult> GetCalendar(int days = 7, DateTime? startDate = null)
        {
            if (days <= 0) return BadRequest(new { Message = "Parâmetro 'days' deve ser maior que zero." });

            var start = startDate?.Date ?? DateTime.Today;
            var end = start.AddDays(days).AddTicks(-1); // fim do último dia incluído

            var itens = await _agendamentoService.GetAllAgendamentosAsync();

            var eventos = itens
                .Where(a => a.DataHora >= start && a.DataHora <= end)
                .OrderBy(a => a.DataHora)
                .Select(a => new AgendamentoCalendarModel
                {
                    Id = a.Codg,
                    ContatoId = a.ContatoId,
                    ContatoNome = a.Contato?.Nome,
                    Title = string.IsNullOrWhiteSpace(a.Dscr) ? "Sem título" : a.Dscr!,
                    Date = a.DataHora.Date,
                    Time = a.DataHora.ToString("HH:mm"),
                    Description = a.Dscr
                })
                .ToList();

            return Ok(new
            {
                StartDate = start,
                EndDate = end,
                Days = days,
                Total = eventos.Count,
                Events = eventos
            });
        }

        [HttpGet("ListarTodosAgendamentos")]
        public async Task<ActionResult<List<Agendamento>>> GetAllAgendamentos()
        {
            var itens = await _agendamentoService.GetAllAgendamentosAsync();
            return Ok(itens);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Agendamento>> GetAgendamentoById(int id)
        {
            var item = await _agendamentoService.GetAgendamentoByIdAsync(id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        [HttpPost]
        public async Task<ActionResult> AddAgendamento([FromBody] AgendamentoCreateModel model)
        {
            if (model == null) return BadRequest();

            var entity = new Agendamento
            {
                Codg = 0,
                DataHora = model.DataHora,
                Dscr = model.Dscr,
                ContatoId = model.ContatoId
            };

            await _agendamentoService.AddAgendamentoAsync(entity);
            return CreatedAtAction(nameof(GetAgendamentoById), new { id = entity.Codg }, entity);
        }

        [HttpPost("InserirAgendamentos")]
        public async Task<ActionResult> AddAgendamentos([FromBody] List<AgendamentoCreateModel> models)
        {
            if (models == null || models.Count == 0) return BadRequest();

            var entities = models.Select(m => new Agendamento
            {
                Codg = 0,
                DataHora = m.DataHora,
                Dscr = m.Dscr,
                ContatoId = m.ContatoId
            }).ToList();

            await _agendamentoService.AddAgendamentosAsync(entities);
            return Ok(new { Inseridos = entities.Count });
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateAgendamento(int id, [FromBody] AgendamentoCreateModel model)
        {
            if (model == null) return BadRequest();

            var existing = await _agendamentoService.GetAgendamentoByIdAsync(id);
            if (existing == null) return NotFound();

            existing.DataHora = model.DataHora;
            existing.Dscr = model.Dscr;
            existing.ContatoId = model.ContatoId;

            await _agendamentoService.UpdateAgendamentoAsync(existing);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAgendamento(int id)
        {
            var item = await _agendamentoService.GetAgendamentoByIdAsync(id);
            if (item == null) return NotFound();
            await _agendamentoService.DeleteAgendamentoAsync(item);
            return NoContent();
        }
    }
}
