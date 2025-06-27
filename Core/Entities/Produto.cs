namespace Core.Entities
{
    public class Produto : EntityBase
    {

        public required string Nome { get; set; }

        public required string Descricao { get; set; }

        public decimal Preco { get; set; }

        public bool Disponivel { get; set; }

        public required int CategoriaId { get; set; }

        public required Categoria Categoria { get; set; }

        public ICollection<PedidoItem>? PedidoItens { get; set; }

    }
}
