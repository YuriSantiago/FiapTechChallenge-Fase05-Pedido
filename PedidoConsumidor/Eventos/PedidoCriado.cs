using Core.Interfaces.Services;
using Core.Requests.Create;
using MassTransit;

namespace PedidoConsumidor.Eventos
{
    public class PedidoCriado : IConsumer<PedidoRequest>
    {

        private readonly IPedidoService _pedidoService;

        public PedidoCriado(IPedidoService pedidoService)
        {
            _pedidoService = pedidoService;
        }

        public Task Consume(ConsumeContext<PedidoRequest> context)
        {
            _pedidoService.Create(context.Message);
            return Task.CompletedTask;
        }

    }
}
