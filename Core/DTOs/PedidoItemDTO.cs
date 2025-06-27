using Core.Entities;

namespace Core.DTOs
{
    public class PedidoItemDTO
    {

        public required int ProdutoId { get; set; }

        public required int Quantidade { get; set; }

        public required decimal PrecoTotal { get; set; }

        public required ProdutoDTO Produto { get; set; }
    }
}
