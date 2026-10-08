using Novvi.Application.DTO;

namespace Novvi.Application.Interfaces.Services;

public interface IUsuarioService
{
    IReadOnlyList<UsuarioResponse> GetAll();
    UsuarioResponse GetById(Guid id);
    UsuarioResponse Create(UsuarioRequest request);
    UsuarioResponse Update(Guid id, UsuarioRequest request);
    UsuarioResponse AdicionarEndereco(Guid id, EnderecoRequest request);
    void Delete(Guid id);
}
