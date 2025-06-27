namespace Core.DTOs
{
    public class ProdutoDTO
    {

        public required string Nome { get; set; }

        public required string Descricao { get; set; }

        public decimal Preco { get; set; }

        public bool Disponivel { get; set; }

        public required CategoriaDTO Categoria { get; set; }

    }
}
