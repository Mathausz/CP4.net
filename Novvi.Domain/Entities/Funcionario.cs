using Novvi.Domain.Commons;
using Novvi.Domain.Exceptions;

namespace Novvi.Domain.Entities;

/// <summary>Funcionário responsável por atender pedidos. 1:N -&gt; Pedido.</summary>
public class Funcionario : EntidadeBase
{
    public string Nome { get; private set; }
    public double Salario { get; private set; }

    public List<Pedido> PedidosRealizados { get; private set; } = new();

    protected Funcionario() { }

    public Funcionario(string nome, double salario)
    {
        if (string.IsNullOrWhiteSpace(nome) || nome.Length < 2)
            throw new DomainException("Nome incompleto");
        Nome = nome;

        if (salario < 1)
            throw new DomainException("Salario inválido");
        Salario = salario;
    }

    /// <summary>Atualiza os dados cadastrais do funcionário.</summary>
    public void Atualizar(string nome, double salario)
    {
        if (string.IsNullOrWhiteSpace(nome) || nome.Length < 2)
            throw new DomainException("Nome incompleto");
        Nome = nome;

        if (salario < 1)
            throw new DomainException("Salario inválido");
        Salario = salario;
    }

    public override string ToString() => $"Funcionario: {Nome} - {Salario}";
}
