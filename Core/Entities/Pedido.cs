using Core.Enums;

namespace Core.Entities
{
    public class Pedido : EntityBase
    {

        public required int UsuarioId { get; set; }

        public required decimal PrecoTotal { get; set; }

        public required StatusPedido Status { get; set; } = StatusPedido.Pendente;

        public required string TipoEntrega { get; set; }

        public string? DescricaoCancelamento { get; set; }

        public PedidoControleCozinha? PedidoControleCozinha { get; set; }

        public required Usuario Usuario { get; set; }

        public ICollection<PedidoItem> Itens { get; set; } = [];

    }

   
}
