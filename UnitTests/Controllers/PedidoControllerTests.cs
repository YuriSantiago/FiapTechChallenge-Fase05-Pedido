using Core.DTOs;
using Core.Entities;
using Core.Interfaces.Services;
using Core.Requests.Create;
using Core.Requests.Delete;
using Core.Requests.Update;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using PedidoProdutor.Controllers;

namespace UnitTests.Controllers
{
    public class PedidoControllerTests
    {
        private readonly Mock<IBus> _mockBus;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<IPedidoService> _mockPedidoService;
        private readonly PedidoController _pedidoController;

        public PedidoControllerTests()
        {
            _mockBus = new Mock<IBus>();
            _mockConfiguration = new Mock<IConfiguration>();
            _mockPedidoService = new Mock<IPedidoService>();
            _pedidoController = new PedidoController(_mockBus.Object, _mockConfiguration.Object, _mockPedidoService.Object);
        }

        [Fact]
        public void GetAll_ShouldReturnOkWithPedidos()
        {
            // Arrange
            var pedidos = new List<PedidoDTO>
            {
               new() {
                    Id = 1,
                    DataInclusao = DateTime.Now,
                    PrecoTotal = 50.00M,
                    Status = "Pendente",
                    TipoEntrega = "DELIVERY",
                    Usuario = new UsuarioDTO {Nome = "Yuri", Role = "ADMIN"}
               }
             };

            _mockPedidoService.Setup(s => s.GetAll()).Returns(pedidos);

            // Act
            var result = _pedidoController.Get();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal(pedidos, okResult.Value);
        }

        [Fact]
        public void GetAll_ShouldReturnBadRequest_WhenServiceThrowsException()
        {
            // Arrange
            _mockPedidoService.Setup(s => s.GetAll()).Throws(new Exception("Erro inesperado"));

            // Act
            var result = _pedidoController.Get();

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var value = badRequest.Value?.GetType().GetProperty("mensagem");
            Assert.NotNull(value);
            Assert.Equal("Erro inesperado", value.GetValue(badRequest.Value)?.ToString());
        }

        [Fact]
        public void GetById_ShouldReturnOkWithPedido()
        {
            // Arrange
            var pedido = new PedidoDTO
            {
                Id = 1,
                DataInclusao = DateTime.Now,
                PrecoTotal = 50.00M,
                Status = "Pendente",
                TipoEntrega = "DELIVERY",
                Usuario = new UsuarioDTO { Nome = "Yuri", Role = "ADMIN" }
            };

            _mockPedidoService.Setup(s => s.GetById(1)).Returns(pedido);

            // Act
            var result = _pedidoController.Get(1);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, ok.StatusCode);
            Assert.Equal(pedido, ok.Value);
        }

        [Fact]
        public void GetById_ShouldReturnBadRequest_WhenServiceThrowsException()
        {
            // Arrange
            _mockPedidoService.Setup(s => s.GetById(It.IsAny<int>())).Throws(new Exception("Erro ao buscar ID"));

            // Act
            var result = _pedidoController.Get(1);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var value = badRequest.Value?.GetType().GetProperty("mensagem");
            Assert.NotNull(value);
            Assert.Equal("Erro ao buscar ID", value.GetValue(badRequest.Value)?.ToString());
        }

        [Fact]
        public async Task Post_ShouldReturnOk_WhenPedidoRequestIsValid()
        {
            // Arrange
            var pedidoRequest = new PedidoRequest
            {
                UsuarioId = 1,
                TipoEntrega = "DELIVERY",
                Itens = [new PedidoItemRequest { ProdutoId = 1, Quantidade = 2 }]
            };

            var endpointMock = new Mock<ISendEndpoint>();

            _mockPedidoService.Setup(s => s.GetAll()).Returns([]);
            _mockBus.Setup(b => b.GetSendEndpoint(It.IsAny<Uri>())).ReturnsAsync(endpointMock.Object);
            _mockConfiguration.Setup(c => c.GetSection("MassTransit:Queues")["PedidoCadastroQueue"]).Returns("filaCadastroPedido");

            // Act
            var result = await _pedidoController.Post(pedidoRequest);

            // Assert
            var ok = Assert.IsType<OkResult>(result);
            endpointMock.Verify(e => e.Send(pedidoRequest, default), Times.Once);
        }

        [Fact]
        public async Task Post_ShouldReturnBadRequest_WhenQueueFails()
        {
            // Arrange
            var pedidoRequest = new PedidoRequest
            {
                UsuarioId = 1,
                TipoEntrega = "DELIVERY",
                Itens = [new PedidoItemRequest { ProdutoId = 1, Quantidade = 2 }]
            };

            _mockPedidoService.Setup(s => s.GetAll()).Returns([]);
            _mockConfiguration.Setup(c => c.GetSection("MassTransit:Queues")["PedidoCadastroQueue"]).Returns("filaCadastroPedido");
            _mockBus.Setup(b => b.GetSendEndpoint(It.IsAny<Uri>())).ThrowsAsync(new Exception("Falha no RabbitMQ"));

            // Act
            var result = await _pedidoController.Post(pedidoRequest);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var value = badRequest.Value?.GetType().GetProperty("mensagem");
            Assert.NotNull(value);
            Assert.Equal("Falha no RabbitMQ", value.GetValue(badRequest.Value)?.ToString());
        }

        [Fact]
        public async Task Cancel_ShouldReturnOk_WhenCancelationIsValid()
        {
            // Arrange
            var pedidoCancelationRequest = new PedidoCancelationRequest
            {
                Id = 1,
                DescricaoCancelamento = "Pedido cancelado pelo cliente"
            };

            var endpointMock = new Mock<ISendEndpoint>();

            _mockPedidoService.Setup(s => s.VerifyPossibilityToCancel(1)).Returns(true);
            _mockConfiguration.Setup(c => c.GetSection("MassTransit:Queues")["PedidoCancelamentoQueue"]).Returns("filaCancelamentoPedido");
            _mockBus.Setup(b => b.GetSendEndpoint(It.IsAny<Uri>())).ReturnsAsync(endpointMock.Object);

            // Act
            var result = await _pedidoController.Cancel(pedidoCancelationRequest);

            // Assert
            var okResult = Assert.IsType<OkResult>(result);
            endpointMock.Verify(e => e.Send(pedidoCancelationRequest, default), Times.Once);
        }

        [Fact]
        public async Task Cancel_ShouldReturnBadRequest_WhenQueueFails()
        {
            // Arrange
            var pedidoCancelationRequest = new PedidoCancelationRequest
            {
                Id = 1,
                DescricaoCancelamento = "Erro interno"
            };

            _mockConfiguration.Setup(c => c.GetSection("MassTransit:Queues")["PedidoCancelamentoQueue"]).Returns("filaCancelamentoPedido");

            _mockBus.Setup(b => b.GetSendEndpoint(It.IsAny<Uri>())).ThrowsAsync(new Exception("Falha na fila"));

            // Act
            var result = await _pedidoController.Cancel(pedidoCancelationRequest);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var value = badRequest.Value?.GetType().GetProperty("mensagem");
            Assert.NotNull(value);
            Assert.Equal("Pedido não pode mais ser cancelado!", value.GetValue(badRequest.Value)?.ToString());
        }

        [Fact]
        public async Task Delete_ShouldReturnOk_WhenIdIsValid()
        {
            // Arrange
            int id = 1;

            var endpointMock = new Mock<ISendEndpoint>();
            _mockBus.Setup(b => b.GetSendEndpoint(It.IsAny<Uri>())).ReturnsAsync(endpointMock.Object);
            _mockConfiguration.Setup(c => c.GetSection("MassTransit:Queues")["PedidoCadastroQueue"]).Returns("filaCadastroPedido");

            // Act
            var result = await _pedidoController.Delete(id);

            // Assert
            var ok = Assert.IsType<OkResult>(result);
            endpointMock.Verify(e => e.Send(It.Is<PedidoDeleteRequest>(r => r.Id == id), default), Times.Once);
        }

        [Fact]
        public async Task Delete_ShouldReturnBadRequest_WhenQueueFails()
        {
            // Arrange
            int id = 1;

            _mockConfiguration.Setup(c => c.GetSection("MassTransit:Queues")["PedidoCadastroQueue"]).Returns("filaCadastroPedido");
            _mockBus.Setup(b => b.GetSendEndpoint(It.IsAny<Uri>())).ThrowsAsync(new Exception("Falha ao deletar"));

            // Act
            var result = await _pedidoController.Delete(id);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var value = badRequest.Value?.GetType().GetProperty("mensagem");
            Assert.NotNull(value);
            Assert.Equal("Falha ao deletar", value.GetValue(badRequest.Value)?.ToString());
        }
    }
}
