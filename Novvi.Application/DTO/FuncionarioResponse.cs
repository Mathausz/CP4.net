using Novvi.Domain.Entities;

namespace Novvi.Application.DTO;

/// <summary>Dados de um funcionário retornados pela API.</summary>
public record FuncionarioResponse(Guid Id, string Nome, double Salario, DateTime CriadoEm)
{
    public static FuncionarioResponse FromDomain(Funcionario funcionario) => new(
        funcionario.Id,
        funcionario.Nome,
        funcionario.Salario,
        funcionario.CriadoEm);
}
