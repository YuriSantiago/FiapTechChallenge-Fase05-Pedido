using Core.Entities;
using Core.Interfaces.Repositories;

namespace Infrastructure.Repositories
{
    public class ProdutoRepository : BaseRepository<Produto>, IProdutoRepository
    {

        public ProdutoRepository(ApplicationDbContext context) : base(context)
        {
        }

        public IList<Produto> GetAllByCategory(short categoria)
        {
            return [.. _context.Produtos.Where(r => r.CategoriaId == categoria)];
        }

    }
}
