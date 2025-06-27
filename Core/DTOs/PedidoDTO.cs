namespace Core.DTOs
{
    public class PedidoDTO
    {

        public int Id { get; set; }

        public DateTime DataInclusao { get; set; }

        public required decimal PrecoTotal { get; set; }

        public required string Status { get; set; }

        public required string TipoEntrega { get; set; }

        public string? DescricaoCancelamento { get; set; }

        public required UsuarioDTO Usuario { get; set; }

        public ICollection<PedidoItemDTO> Itens { get; set; } = [];
    }
}
