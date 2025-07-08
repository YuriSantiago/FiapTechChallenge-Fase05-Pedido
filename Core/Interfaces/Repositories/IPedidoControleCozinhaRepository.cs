using Core.Entities;

namespace Core.Interfaces.Repositories
{

    public interface IPedidoControleCozinhaRepository : IRepository<PedidoControleCozinha>
    {
        PedidoControleCozinha? GetByPedidoId(int pedidoId);

    }

}
