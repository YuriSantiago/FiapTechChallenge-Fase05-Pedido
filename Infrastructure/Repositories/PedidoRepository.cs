using Core.Entities;
using Core.Interfaces.Repositories;

namespace Infrastructure.Repositories
{
    public class PedidoRepository : BaseRepository<Pedido>, IPedidoRepository
    {

        public PedidoRepository(ApplicationDbContext context) : base(context)
        {

        }

    }
}
