using Domain.Interfaces;
using Entities.Entities;
using Infraestructure.Configuration;

namespace Infraestructure.Repository
{
    public class ContatoRepository : RepositoryGenerics<Contato>, IContatoRepository
    {
        public ContatoRepository(ContextBase context) : base(context)
        {
        }
    }
}
