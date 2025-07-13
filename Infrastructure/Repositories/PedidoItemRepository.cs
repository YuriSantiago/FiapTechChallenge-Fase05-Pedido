using Core.Entities;
using Core.Interfaces.Repositories;

namespace Infrastructure.Repositories
{
    public class PedidoItemRepository : BaseRepository<PedidoItem>, IPedidoItemRepository
    {

        public PedidoItemRepository(ApplicationDbContext context) : base(context)
        {
        }

    }
}
