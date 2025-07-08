using Core.Enums;

namespace Core.Entities
{
    public class PedidoControleCozinha : EntityBase
    {

        public required int PedidoId { get; set; }

        public required string NomeCliente { get; set; }

        public required StatusPedido Status { get; set; } = StatusPedido.Pendente;

        public required Pedido Pedido { get; set; }

    }
}
