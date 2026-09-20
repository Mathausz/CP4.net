using Novvi.Domain.Entities;

namespace Novvi.Application.DTO;

/// <summary>Dados de um endereço retornados pela API.</summary>
public record EnderecoResponse(Guid Id, string Cep, string Complemento)
{
    public static EnderecoResponse FromDomain(Endereco endereco) =>
        new(endereco.Id, endereco.Cep, endereco.Complemento);
}
