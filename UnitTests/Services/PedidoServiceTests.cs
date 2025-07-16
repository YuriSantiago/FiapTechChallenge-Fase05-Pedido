using Core.Entities;
using Core.Enums;
using Core.Helpers;
using Core.Interfaces.Repositories;
using Core.Requests.Create;
using Core.Requests.Update;
using Core.Services;
using Moq;

namespace UnitTests.Services
{
    public class PedidoServiceTests
    {
        private readonly Mock<IPedidoControleCozinhaRepository> _pedidoControleCozinhaRepositoryMock;
        private readonly Mock<IPedidoItemRepository> _pedidoItemRepositoryMock;
        private readonly Mock<IPedidoRepository> _pedidoRepositoryMock;
        private readonly Mock<IProdutoRepository> _produtoRepositoryMock;
        private readonly Mock<IUsuarioRepository> _usuarioRepositoryMock;
        private readonly PedidoService _pedidoService;

        public PedidoServiceTests()
        {
            _pedidoControleCozinhaRepositoryMock = new Mock<IPedidoControleCozinhaRepository>();
            _pedidoItemRepositoryMock = new Mock<IPedidoItemRepository>();
            _pedidoRepositoryMock = new Mock<IPedidoRepository>();
            _produtoRepositoryMock = new Mock<IProdutoRepository>();
            _usuarioRepositoryMock = new Mock<IUsuarioRepository>();
            _pedidoService = new PedidoService(_pedidoControleCozinhaRepositoryMock.Object, _pedidoRepositoryMock.Object, _pedidoItemRepositoryMock.Object, _produtoRepositoryMock.Object, _usuarioRepositoryMock.Object);
        }

        [Fact]
        public void GetAll_ShouldReturnListOfPedidoDTO()
        {
            // Arrange
            var pedidos = new List<Pedido>
            {
               new() {
                    Id = 1,
                    DataInclusao = DateTime.Now,
                    UsuarioId = 1,
                    PrecoTotal = 50.00M,
                    Status = StatusPedido.Pendente,
                    TipoEntrega = "DELIVERY",
                    Usuario = new Usuario {Id = 1, Nome = "Yuri", Email = "yuri@email.com", Senha = Base64Helper.Encode("yuri"), Role = "ADMIN"}
               }
             };

            _pedidoRepositoryMock.Setup(r => r.GetAll(It.IsAny<Func<IQueryable<Pedido>, IQueryable<Pedido>>>())).Returns(pedidos);

            // Act
            var result = _pedidoService.GetAll();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(pedidos.Count, result.Count);
        }

        [Fact]
        public void GetById_ShouldReturnPedidoDTO_WhenIdExists()
        {
            // Arrange
            var pedido = new Pedido
            {
                Id = 1,
                DataInclusao = DateTime.Now,
                UsuarioId = 1,
                PrecoTotal = 50.00M,
                Status = StatusPedido.Pendente,
                TipoEntrega = "DELIVERY",
                Usuario = new Usuario { Id = 1, Nome = "Yuri", Email = "yuri@email.com", Senha = Base64Helper.Encode("yuri"), Role = "ADMIN" }
            };

            _pedidoRepositoryMock.Setup(repo => repo.GetById(It.Is<int>(id => id == 1), It.IsAny<Func<IQueryable<Pedido>, IQueryable<Pedido>>>())).Returns(pedido);

            // Act
            var result = _pedidoService.GetById(pedido.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(pedido.Id, result.Id);
        }

        [Fact]
        public void Create_ShouldCreatePedidoAndItens_WhenRequestIsValid()
        {
            // Arrange
            var usuario = new Usuario
            {
                Id = 1,
                Nome = "Yuri",
                Email = "yuri@email.com",
                Senha = Base64Helper.Encode("yuri"),
                Role = "ADMIN"
            };

            var produto = new Produto
            {
                Id = 1,
                DataInclusao = DateTime.Now,
                Nome = "Queijo Quente",
                Descricao = "Pão de forma com uma fatia de queijo",
                Preco = 10.00M,
                Disponivel = true,
                CategoriaId = 1,
                Categoria = new Categoria { Id = 1, Descricao = "LANCHE" }
            };

            var pedidoRequest = new PedidoRequest
            {
                UsuarioId = 1,
                TipoEntrega = "DELIVERY",
                Itens = [new PedidoItemRequest { ProdutoId = 1, Quantidade = 2 }]
            };

            _usuarioRepositoryMock.Setup(r => r.GetById(1)).Returns(usuario);
            _produtoRepositoryMock.Setup(r => r.GetById(1)).Returns(produto);

            var pedidoRepositoryMock = _pedidoRepositoryMock.Setup(r => r.Create(It.IsAny<Pedido>()));
            var controleMock = _pedidoControleCozinhaRepositoryMock.Setup(r => r.Create(It.IsAny<PedidoControleCozinha>()));

            // Act
            _pedidoService.Create(pedidoRequest);

            // Assert
            _pedidoRepositoryMock.Verify(r => r.Create(It.IsAny<Pedido>()), Times.Once);
            _pedidoItemRepositoryMock.Verify(r => r.Create(It.IsAny<PedidoItem>()), Times.Once);
        }

        [Theory]
        [InlineData(StatusPedido.Pendente, true)]
        [InlineData(StatusPedido.Aceito, false)]
        [InlineData(StatusPedido.Rejeitado, false)]
        [InlineData(StatusPedido.Preparando, false)]
        [InlineData(StatusPedido.Pronto, false)]
        [InlineData(StatusPedido.Cancelado, false)]
        public void VerifyPossibilityToCancel_ShouldReturnExpectedResult(StatusPedido status, bool expected)
        {
            // Arrange
            var pedido = new Pedido
            {
                Id = 1,
                DataInclusao = DateTime.Now,
                UsuarioId = 1,
                PrecoTotal = 50.00M,
                Status = status,
                TipoEntrega = "DELIVERY",
                Usuario = new Usuario { Id = 1, Nome = "Yuri", Email = "yuri@email.com", Senha = Base64Helper.Encode("yuri"), Role = "ADMIN" }
            };

            _pedidoRepositoryMock.Setup(r => r.GetById(1)).Returns(pedido);

            // Act
            var result = _pedidoService.VerifyPossibilityToCancel(1);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void Cancel_ShouldUpdatePedidoAndControle_WhenExists()
        {
            // Arrange
            var usuario = new Usuario
            {
                Id = 1,
                Nome = "Yuri Santiago",
                Email = "yuri@email.com",
                Senha = Base64Helper.Encode("yuri"),
                Role = "ADMIN"
            };

            var pedido = new Pedido
            {
                Id = 1,
                DataInclusao = DateTime.Now,
                UsuarioId = 1,
                PrecoTotal = 50.00M,
                Status = StatusPedido.Pendente,
                TipoEntrega = "DELIVERY",
                Usuario = usuario
            };

            var pedidoControleCozinha = new PedidoControleCozinha
            {
                Id = 1,
                DataInclusao = DateTime.Now,
                PedidoId = 1, 
                NomeCliente = usuario.Nome,
                Status = StatusPedido.Pendente,
                Pedido = pedido
            };

            var pedidoCancelationRequest = new PedidoCancelationRequest
            {
                Id = 1,
                DescricaoCancelamento = "Cliente desistiu"
            };

            _pedidoRepositoryMock.Setup(r => r.GetById(1)).Returns(pedido);
            _pedidoControleCozinhaRepositoryMock.Setup(r => r.GetByPedidoId(1)).Returns(pedidoControleCozinha);

            // Act
            _pedidoService.Cancel(pedidoCancelationRequest);

            // Assert
            _pedidoRepositoryMock.Verify(r => r.Update(It.IsAny<Pedido>()), Times.Once);
            _pedidoControleCozinhaRepositoryMock.Verify(r => r.Update(It.IsAny<PedidoControleCozinha>()), Times.Once);
        }

        [Fact]
        public void Delete_ShouldCallRepositoryDelete_WhenIdExists()
        {
            // Arrange
            var id = 1;

            // Act
            _pedidoService.Delete(id);

            // Assert
            _pedidoRepositoryMock.Verify(repo => repo.Delete(id), Times.Once);
        }

    }
}
