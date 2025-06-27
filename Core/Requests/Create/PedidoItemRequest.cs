namespace Core.Requests.Create
{
    public class PedidoItemRequest
    {

        public required int ProdutoId { get; set; }

        public required int Quantidade { get; set; }

    }
}
