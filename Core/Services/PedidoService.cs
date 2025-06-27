using Core.DTOs;
using Core.Entities;
using Core.Interfaces.Repositories;
using Core.Interfaces.Services;
using Core.Requests.Create;
using Core.Requests.Update;
using Microsoft.EntityFrameworkCore;

namespace Core.Services
{
    public class PedidoService : IPedidoService
    {

        private readonly IPedidoRepository _pedidoRepository;
        private readonly IPedidoItemRepository _pedidoItemRepository;
        private readonly IProdutoRepository _produtoRepository;
        private readonly IUsuarioRepository _usuarioRepository;

        public PedidoService(IPedidoRepository pedidoRepository, IPedidoItemRepository pedidoItemRepository, IProdutoRepository produtoRepository, IUsuarioRepository usuarioRepository)
        {
            _pedidoRepository = pedidoRepository;
            _pedidoItemRepository = pedidoItemRepository;
            _produtoRepository = produtoRepository;
            _usuarioRepository = usuarioRepository;
        }

        public IList<PedidoDTO> GetAll()
        {
            var pedidosDTO = new List<PedidoDTO>();
            var pedidos = _pedidoRepository.GetAll(p => p.Include(u => u.Usuario)
                                                         .Include(i => i.Itens)
                                                         .ThenInclude(pro => pro.Produto)
                                                         .ThenInclude(c => c.Categoria));

            foreach (var pedido in pedidos)
            {
                var pedidoItensDTO = new List<PedidoItemDTO>();

                foreach (var item in pedido.Itens)
                {
                    pedidoItensDTO.Add(new PedidoItemDTO()
                    {
                        ProdutoId = item.ProdutoId,
                        Quantidade = item.Quantidade,
                        PrecoTotal = item.PrecoTotal,
                        Produto = new ProdutoDTO()
                        {
                            Nome = item.Produto.Nome,
                            Descricao = item.Produto.Descricao,
                            Preco = item.Produto.Preco,
                            Disponivel = item.Produto.Disponivel,
                            Categoria = new CategoriaDTO()
                            {
                                Descricao = item.Produto.Categoria.Descricao,
                            }
                        }
                    });
                }

                pedidosDTO.Add(new PedidoDTO()
                {
                    Id = pedido.Id,
                    DataInclusao = pedido.DataInclusao,
                    PrecoTotal = pedido.PrecoTotal,
                    Status = pedido.Status.ToString(),
                    TipoEntrega = pedido.TipoEntrega,
                    Usuario = new UsuarioDTO()
                    {
                        Nome = pedido.Usuario.Nome,
                        Role = pedido.Usuario.Role
                    },
                    Itens = pedidoItensDTO
                });
            }

            return pedidosDTO;
        }

        public PedidoDTO GetById(int id)
        {
            var pedido = _pedidoRepository.GetById(id, p => p.Include(u => u.Usuario)
                                                         .Include(i => i.Itens)
                                                         .ThenInclude(pro => pro.Produto)
                                                         .ThenInclude(c => c.Categoria));

            var pedidoItensDTO = new List<PedidoItemDTO>();

            foreach (var item in pedido.Itens)
            {
                pedidoItensDTO.Add(new PedidoItemDTO()
                {
                    ProdutoId = item.ProdutoId,
                    Quantidade = item.Quantidade,
                    PrecoTotal = item.PrecoTotal,
                    Produto = new ProdutoDTO()
                    {
                        Nome = item.Produto.Nome,
                        Descricao = item.Produto.Descricao,
                        Preco = item.Produto.Preco,
                        Disponivel = item.Produto.Disponivel,
                        Categoria = new CategoriaDTO()
                        {
                            Descricao = item.Produto.Categoria.Descricao,
                        }
                    }
                });
            }

            var pedidoDTO = new PedidoDTO()
            {
                Id = pedido.Id,
                DataInclusao = pedido.DataInclusao,
                PrecoTotal = pedido.PrecoTotal,
                Status = pedido.Status.ToString(),
                TipoEntrega = pedido.TipoEntrega,
                Usuario = new UsuarioDTO()
                {
                    Nome = pedido.Usuario.Nome,
                    Role = pedido.Usuario.Role
                },
                Itens = pedidoItensDTO
            };

            return pedidoDTO;
        }

        public void Create(PedidoRequest pedidoRequest)
        {
            var usuario = _usuarioRepository.GetById(pedidoRequest.UsuarioId);
            decimal precoTotal = 0;

            foreach (var item in pedidoRequest.Itens)
            {
                var produto = _produtoRepository.GetById(item.ProdutoId);
                precoTotal += produto.Preco * item.Quantidade;
            }

            var pedido = new Pedido()
            {
                UsuarioId = usuario.Id,
                PrecoTotal = precoTotal,
                Status = StatusPedido.Pendente,
                TipoEntrega = pedidoRequest.TipoEntrega,
                Usuario = usuario
            };

            _pedidoRepository.Create(pedido);

            IncluirItensPedido(pedido, pedidoRequest.Itens);
        }

        public void Cancel(PedidoCancelationRequest pedidoCancelationRequest)
        {
            var pedido = _pedidoRepository.GetById(pedidoCancelationRequest.Id);

            pedido.Status = StatusPedido.Cancelado;
            pedido.DescricaoCancelamento = pedidoCancelationRequest.DescricaoCancelamento;

            _pedidoRepository.Update(pedido);
        }

        public void Delete(int id)
        {
            _pedidoRepository.Delete(id);
        }

        private void IncluirItensPedido(Pedido pedido, List<PedidoItemRequest> itens)
        {

            foreach (var item in itens)
            {
                var produto = _produtoRepository.GetById(item.ProdutoId);

                var pedidoItem = new PedidoItem()
                {
                    PedidoId = pedido.Id,
                    ProdutoId = item.ProdutoId,
                    Quantidade = item.Quantidade,
                    PrecoTotal = item.Quantidade * produto.Preco,
                    Pedido = pedido,
                    Produto = produto
                };

                _pedidoItemRepository.Create(pedidoItem);

            }

        }
    }

}
