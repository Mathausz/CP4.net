using Novvi.Domain.Entities;

namespace Novvi.Application.DTO;

/// <summary>Dados de um usuário retornados pela API.</summary>
public record UsuarioResponse(
    Guid Id,
    string Nome,
    string Email,
    List<EnderecoResponse> Enderecos,
    DateTime CriadoEm)
{
    public static UsuarioResponse FromDomain(Usuario usuario) => new(
        usuario.Id,
        usuario.Nome,
        usuario.Email,
        usuario.Enderecos.Select(EnderecoResponse.FromDomain).ToList(),
        usuario.CriadoEm);
}
