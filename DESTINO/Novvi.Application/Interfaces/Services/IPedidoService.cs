using Novvi.Application.DTO;

namespace Novvi.Application.Interfaces.Services;

public interface IPedidoService
{
    /// <summary>Contrato antigo (v1, deprecada): lista inteira, sem paginação.</summary>
    IReadOnlyList<PedidoResponse> GetAll();

    /// <summary>Contrato novo (v2): envelope paginado. Valida page/pageSize (CP5).</summary>
    PagedResponse<PedidoResponse> GetPaged(int page, int pageSize);
    PedidoResponse GetById(Guid id);
    PedidoResponse Create(PedidoRequest request);
    PedidoResponse RegistrarPagamento(Guid id, PagamentoRequest request);
    void Delete(Guid id);
}
