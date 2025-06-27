namespace Core.Entities
{
    public class PedidoItem : EntityBase
    {

        public required int PedidoId { get; set; }

        public required int ProdutoId { get; set; }

        public required int Quantidade { get; set; }

        public required decimal PrecoTotal { get; set; }

        public required Produto Produto { get; set; }

        public required Pedido Pedido { get; set; }



    }
}
