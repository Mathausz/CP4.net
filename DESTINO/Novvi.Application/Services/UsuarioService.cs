using Novvi.Application.DTO;
using Novvi.Application.Interfaces.Repositories;
using Novvi.Application.Interfaces.Services;
using Novvi.Domain.Entities;
using Novvi.Domain.Exceptions;

namespace Novvi.Application.Services;

public class UsuarioService(IUsuarioRepository usuarioRepository) : IUsuarioService
{
    public IReadOnlyList<UsuarioResponse> GetAll() =>
        usuarioRepository.GetAllCompleto()
            .Select(UsuarioResponse.FromDomain)
            .ToList();

    public UsuarioResponse GetById(Guid id)
    {
        var usuario = BuscarOuFalhar(id);
        return UsuarioResponse.FromDomain(usuario);
    }

    public UsuarioResponse Create(UsuarioRequest request)
    {
        var enderecos = new List<Endereco>();
        var usuario = new Usuario(request.Nome, request.Email);

        usuarioRepository.Add(usuario);

        foreach (var enderecoRequest in request.Enderecos)
        {
            var endereco = new Endereco(enderecoRequest.Cep, enderecoRequest.Complemento, usuario.Id);
            usuario.AdicionarEndereco(endereco);
        }

        usuarioRepository.Update(usuario);

        return UsuarioResponse.FromDomain(usuario);
    }

    public UsuarioResponse Update(Guid id, UsuarioRequest request)
    {
        var usuario = BuscarOuFalhar(id);
        usuario.Atualizar(request.Nome, request.Email);
        usuarioRepository.Update(usuario);
        return UsuarioResponse.FromDomain(usuario);
    }

    public UsuarioResponse AdicionarEndereco(Guid id, EnderecoRequest request)
    {
        var usuario = BuscarOuFalhar(id);
        var endereco = new Endereco(request.Cep, request.Complemento, usuario.Id);
        usuario.AdicionarEndereco(endereco);
        usuarioRepository.Update(usuario);
        return UsuarioResponse.FromDomain(usuario);
    }

    public void Delete(Guid id)
    {
        if (!usuarioRepository.Delete(id))
            throw new NotFoundException($"Usuário {id} não encontrado");
    }

    private Usuario BuscarOuFalhar(Guid id) =>
        usuarioRepository.GetByIdCompleto(id)
        ?? throw new NotFoundException($"Usuário {id} não encontrado");
}
