using Core.Entities;

namespace Core.Requests.Update
{
    public class PedidoCancelationRequest
    {
        public required int Id { get; set; }

        public required string DescricaoCancelamento { get; set; }

    }
}
