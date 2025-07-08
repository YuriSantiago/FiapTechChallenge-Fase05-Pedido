using Core.DTOs;
using Core.Requests.Create;
using Core.Requests.Update;

namespace Core.Interfaces.Services
{
    public interface IPedidoService
    {

        IList<PedidoDTO> GetAll();

        PedidoDTO GetById(int id);

        void Create(PedidoRequest pedidoRequest);

        bool VerifyPossibilityToCancel(int id);

        void Cancel(PedidoCancelationRequest pedidoCancelationRequest);

        void Delete(int id);

    }
}
