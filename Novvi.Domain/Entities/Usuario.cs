using Novvi.Domain.Commons;
using Novvi.Domain.Exceptions;

namespace Novvi.Domain.Entities;

/// <summary>Usuário/cliente da plataforma. 1:N -&gt; Endereco | 1:N -&gt; Pedido</summary>
public class Usuario : EntidadeBase
{
    public string Nome { get; private set; }
    public string Email { get; private set; }

    public List<Endereco> Enderecos { get; private set; } = new();
    public List<Pedido> HistoricoPedidos { get; private set; } = new();

    protected Usuario() { }

    public Usuario(string nome, string email, List<Endereco>? enderecos = null)
    {
        if (string.IsNullOrWhiteSpace(nome) || nome.Length < 2)
            throw new DomainException("Nome inválido");
        Nome = nome;

        if (string.IsNullOrWhiteSpace(email) || email.Length < 5 || !email.Contains('@'))
            throw new DomainException("Email inválido");
        Email = email;

        if (enderecos is not null)
            Enderecos = enderecos;
    }

    /// <summary>Atualiza os dados cadastrais do usuário.</summary>
    public void Atualizar(string nome, string email)
    {
        if (string.IsNullOrWhiteSpace(nome) || nome.Length < 2)
            throw new DomainException("Nome inválido");
        Nome = nome;

        if (string.IsNullOrWhiteSpace(email) || email.Length < 5 || !email.Contains('@'))
            throw new DomainException("Email inválido");
        Email = email;
    }

    /// <summary>Adiciona um novo endereço ao usuário.</summary>
    public void AdicionarEndereco(Endereco endereco) => Enderecos.Add(endereco);

    public string MostrarHistoricoPedidos() =>
        "Historico Pedidos: " + string.Join(" | ", HistoricoPedidos.Select(p => p.ToString()));

    public override string ToString() => $"Nome: {Nome}, Email: {Email}";
}
