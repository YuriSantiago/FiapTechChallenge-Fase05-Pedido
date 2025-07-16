using Core.DTOs;
using Core.Requests.Create;
using Core.Requests.Delete;
using Core.Requests.Update;
using System.Net;
using System.Net.Http.Json;

namespace IntegrationTests.Controllers
{
    public class PedidoControllerTests : IClassFixture<CustomWebApplicationFactory<PedidoProdutor.Program>>
    {
        private readonly HttpClient _client;

        public PedidoControllerTests(CustomWebApplicationFactory<PedidoProdutor.Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetAll_ShouldReturnOk()
        {
            // Act
            var response = await _client.GetAsync("/Pedido");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var pedidos = await response.Content.ReadFromJsonAsync<List<PedidoDTO>>();
            Assert.NotNull(pedidos);
            Assert.True(pedidos.Count >= 0);
        }

        [Fact]
        public async Task GetById_ShouldReturnPedido_WhenIdExists()
        {
            // Arrange
            int pedidoId = 1;

            // Act
            var response = await _client.GetAsync($"/Pedido/{pedidoId}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var pedido = await response.Content.ReadFromJsonAsync<PedidoDTO>();
            Assert.NotNull(pedido);
            Assert.Equal(pedidoId, pedido.Id);
        }

        [Fact]
        public async Task GetById_ShouldReturnNotFound_WhenIdDoesNotExist()
        {
            // Arrange
            int pedidoId = 9999;

            // Act
            var response = await _client.GetAsync($"/Pedido/{pedidoId}");

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Create_ShouldReturnOk_WhenPedidoIsValid()
        {
            // Arrange
            var pedidoRequest = new PedidoRequest()
            {
                UsuarioId = 1,
                TipoEntrega = "DELIVERY",
                Itens = [new PedidoItemRequest { ProdutoId = 1, Quantidade = 2 }]
            };

            // Act
            var response = await _client.PostAsJsonAsync("/Pedido", pedidoRequest);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Create_ShouldReturnBadRequest_WhenPedidoIsInvalid()
        {
            // Arrange
            var pedidoRequest = new PedidoRequest()
            {
                UsuarioId = 1,
                TipoEntrega = "",
                Itens = [new PedidoItemRequest { ProdutoId = 1, Quantidade = 2 }]
            };

            // Act
            var response = await _client.PostAsJsonAsync("/Pedido", pedidoRequest);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Cancel_ShouldReturnOK_WhenPedidoToCancelIsValid()
        {
            // Arrange
            var pedidoCancelationRequest = new PedidoCancelationRequest
            {
                Id = 1,
                DescricaoCancelamento = "Não desejo mais",
            };

            // Act
            var response = await _client.PutAsJsonAsync("/Pedido", pedidoCancelationRequest);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Cancel_ShouldReturnBadRequest_WhenPedidoToCancelIsInvalid()
        {
            // Arrange
            var pedidoCancelationRequest = new PedidoCancelationRequest
            {
                Id = 2,
                DescricaoCancelamento = "Não desejo mais",
            };

            // Act
            var response = await _client.PutAsJsonAsync("/Pedido", pedidoCancelationRequest);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Cancel_ShouldReturnBadrequest_WhenRequestToCancelIsInvalid()
        {
            // Arrange
            var pedidoCancelationRequest = new PedidoCancelationRequest
            {
                Id = 1,
                DescricaoCancelamento = "",
            };

            // Act
            var response = await _client.PutAsJsonAsync("/Pedido", pedidoCancelationRequest);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Delete_ShouldReturnOk_WhenIdExists()
        {
            // Arrange
            var pedidoDeleteRequest = new HttpRequestMessage(HttpMethod.Delete, "/Pedido")
            {
                Content = JsonContent.Create(new PedidoDeleteRequest { Id = 1 })
            };

            // Act
            var response = await _client.SendAsync(pedidoDeleteRequest);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

    }
}
