using Novvi.Application.DTO;

namespace Novvi.Application.Interfaces.Services;

public interface IProdutoService
{
    IReadOnlyList<ProdutoResponse> GetAll();
    ProdutoResponse GetById(Guid id);
    ProdutoResponse Create(ProdutoRequest request);
    ProdutoResponse Update(Guid id, ProdutoRequest request);
    void Delete(Guid id);
}
