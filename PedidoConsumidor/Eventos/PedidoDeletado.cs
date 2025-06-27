using Core.Interfaces.Services;
using Core.Requests.Delete;
using MassTransit;

namespace PedidoConsumidor.Eventos
{
    public class PedidoDeletado : IConsumer<PedidoDeleteRequest>
    {
        private readonly IPedidoService _pedidoService;

        public PedidoDeletado(IPedidoService pedidoService)
        {
            _pedidoService = pedidoService;
        }

        public Task Consume(ConsumeContext<PedidoDeleteRequest> context)
        {
            _pedidoService.Delete(context.Message.Id);
            return Task.CompletedTask;
        }
    }
}
