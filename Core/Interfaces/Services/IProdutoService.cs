using Core.DTOs;
using Core.Requests.Create;
using Core.Requests.Update;

namespace Core.Interfaces.Services
{
    public interface IProdutoService
    {

        IList<ProdutoDTO> GetAll();

        ProdutoDTO GetById(int id);

        IList<ProdutoDTO> GetAllByCategory(short categoria);

        void Create(ProdutoRequest produtoRequest);

        void Put(PedidoCancelationRequest produtoUpdateRequest);

        void Delete(int id);

    }
}
