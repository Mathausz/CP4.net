using Novvi.Application.DTO;
using Novvi.Application.Interfaces.Repositories;
using Novvi.Application.Interfaces.Services;
using Novvi.Domain.Entities;
using Novvi.Domain.Exceptions;

namespace Novvi.Application.Services;

/// <summary>Usa diretamente o repositório genérico IRepository&lt;Funcionario&gt; (CRUD simples).</summary>
public class FuncionarioService(IRepository<Funcionario> funcionarioRepository) : IFuncionarioService
{
    public IReadOnlyList<FuncionarioResponse> GetAll() =>
        funcionarioRepository.GetAll().Select(FuncionarioResponse.FromDomain).ToList();

    public FuncionarioResponse GetById(Guid id) => FuncionarioResponse.FromDomain(BuscarOuFalhar(id));

    public FuncionarioResponse Create(FuncionarioRequest request)
    {
        var funcionario = new Funcionario(request.Nome, request.Salario);
        funcionarioRepository.Add(funcionario);
        return FuncionarioResponse.FromDomain(funcionario);
    }

    public FuncionarioResponse Update(Guid id, FuncionarioRequest request)
    {
        var funcionario = BuscarOuFalhar(id);
        funcionario.Atualizar(request.Nome, request.Salario);
        funcionarioRepository.Update(funcionario);
        return FuncionarioResponse.FromDomain(funcionario);
    }

    public void Delete(Guid id)
    {
        if (!funcionarioRepository.Delete(id))
            throw new NotFoundException($"Funcionário {id} não encontrado");
    }

    private Funcionario BuscarOuFalhar(Guid id) =>
        funcionarioRepository.GetById(id)
        ?? throw new NotFoundException($"Funcionário {id} não encontrado");
}
