using Core.Interfaces.Services;
using Core.Requests.Update;
using MassTransit;

namespace PedidoConsumidor.Eventos
{
    public class PedidoCancelado : IConsumer<PedidoCancelationRequest>
    {

        private readonly IPedidoService _pedidoService;

        public PedidoCancelado(IPedidoService pedidoService)
        {
            _pedidoService = pedidoService;
        }

        public Task Consume(ConsumeContext<PedidoCancelationRequest> context)
        {
            _pedidoService.Cancel(context.Message);
            return Task.CompletedTask;
        }

    }
}
