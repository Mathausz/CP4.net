using Novvi.Application.DTO;

namespace Novvi.Application.Interfaces.Services;

public interface IFuncionarioService
{
    IReadOnlyList<FuncionarioResponse> GetAll();
    FuncionarioResponse GetById(Guid id);
    FuncionarioResponse Create(FuncionarioRequest request);
    FuncionarioResponse Update(Guid id, FuncionarioRequest request);
    void Delete(Guid id);
}
