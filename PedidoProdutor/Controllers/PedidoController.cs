using Core.Interfaces.Services;
using Core.Requests.Create;
using Core.Requests.Delete;
using Core.Requests.Update;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace PedidoProdutor.Controllers
{
    [ApiController]
    [Route("/[controller]")]
    public class PedidoController : ControllerBase
    {
        private readonly IBus _bus;
        private readonly IConfiguration _configuration;
        private readonly IPedidoService _pedidoService;
    

        public PedidoController(IBus bus, IConfiguration configuration, IPedidoService pedidoService)
        {
            _bus = bus;
            _configuration = configuration;
            _pedidoService = pedidoService;
        }

        /// <summary>
        /// Busca todos os pedidos cadastrados
        /// </summary>
        /// <returns>Retorna todos os pedidos cadastrados</returns>
        /// <response code="200">Listagem retornada com sucesso</response>
        /// <response code="400">Erro ao listar os pedidos</response>
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                return Ok(_pedidoService.GetAll());
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }

        /// <summary>
        /// Busca um pedido por Id
        /// </summary>
        /// <param name="id">Id do pedido</param>
        /// <returns>Retorna um pedido filtrado por um Id</returns>
        /// <response code="200">Pedido retornado com sucesso</response>
        /// <response code="400">Erro ao buscar o pedido</response>
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [HttpGet("{id:int}")]
        public IActionResult Get([FromRoute] int id)
        {
            try
            {
                return Ok(_pedidoService.GetById(id));
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }

        /// <summary>
        /// Cadastra um novo pedido 
        /// </summary>
        /// <param name="pedidoRequest">Objeto do tipo "PedidoRequest"</param>
        /// <response code="200">Produto cadastrado com sucesso</response>
        /// <response code="400">Erro ao cadastrar o produto</response>
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] PedidoRequest pedidoRequest)
        {

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {

                var nomeFila = _configuration.GetSection("MassTransit:Queues")["PedidoCadastroQueue"] ?? string.Empty;
                var endpoint = await _bus.GetSendEndpoint(new Uri($"queue:{nomeFila}"));
                await endpoint.Send(pedidoRequest);

                return Ok();
            }

            catch (Exception ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }

        /// <summary>
        /// Cancela o pedido
        /// </summary>
        /// <param name="pedidoCancelationRequest">Objeto do tipo "PedidoCancelationRequest"</param>
        /// <response code="200">Pedido cancelado com sucesso</response>
        /// <response code="400">Erro ao cancelar o pedido</response>
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [HttpPut]
        public async Task<IActionResult> Cancel([FromBody] PedidoCancelationRequest pedidoCancelationRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                if (!_pedidoService.VerifyPossibilityToCancel(pedidoCancelationRequest.Id))
                    throw new Exception("Pedido não pode mais ser cancelado!");

                var nomeFila = _configuration.GetSection("MassTransit:Queues")["PedidoCancelamentoQueue"] ?? string.Empty;
                var endpoint = await _bus.GetSendEndpoint(new Uri($"queue:{nomeFila}"));
                await endpoint.Send(pedidoCancelationRequest);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }

        /// <summary>
        /// Deleta um pedido por Id
        /// </summary>
        /// <param name="id">Id do pedido</param>
        /// <response code="200">Pedido deletado com sucesso</response>
        /// <response code="400">Erro ao deletar o pedido</response>
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {

            try
            {
                var nomeFila = _configuration.GetSection("MassTransit:Queues")["PedidoExclusaoQueue"] ?? string.Empty;
                var endpoint = await _bus.GetSendEndpoint(new Uri($"queue:{nomeFila}"));
                await endpoint.Send(new PedidoDeleteRequest { Id = id });

                return Ok();
            }

            catch (Exception ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }


    }
}
