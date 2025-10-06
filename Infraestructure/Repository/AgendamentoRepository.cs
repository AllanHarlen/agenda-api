using Domain.Interfaces;
using Entities.Entities;
using Infraestructure.Configuration;

namespace Infraestructure.Repository
{
    public class AgendamentoRepository : RepositoryGenerics<Agendamento>, IAgendamentoRepository
    {
        public AgendamentoRepository(ContextBase context) : base(context)
        {
        }
    }
}
