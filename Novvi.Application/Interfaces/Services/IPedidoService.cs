using Novvi.Application.DTO;

namespace Novvi.Application.Interfaces.Services;

public interface IPedidoService
{
    IReadOnlyList<PedidoResponse> GetAll();
    PedidoResponse GetById(Guid id);
    PedidoResponse Create(PedidoRequest request);
    PedidoResponse RegistrarPagamento(Guid id, PagamentoRequest request);
    void Delete(Guid id);
}
