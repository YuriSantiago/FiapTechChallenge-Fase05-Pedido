using Core.Entities;
using Core.Interfaces.Repositories;

namespace Infrastructure.Repositories
{
    public class PedidoControleCozinhaRepository : BaseRepository<PedidoControleCozinha>, IPedidoControleCozinhaRepository
    {

        public PedidoControleCozinhaRepository(ApplicationDbContext context) : base(context)
        {

        }

        public PedidoControleCozinha? GetByPedidoId(int pedidoId)
        {
            return _context.PedidosControleCozinha.Where(r => r.PedidoId == pedidoId).FirstOrDefault();
        }

    }
}
