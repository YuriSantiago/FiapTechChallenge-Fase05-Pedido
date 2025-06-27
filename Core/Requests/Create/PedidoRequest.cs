namespace Core.Requests.Create
{
    public class PedidoRequest
    {

        public required int UsuarioId { get; set; }

        public required string TipoEntrega { get; set; }

        public required List<PedidoItemRequest> Itens {get; set;}

    }
}
