using Novvi.Domain.Commons;
using Novvi.Domain.Exceptions;

namespace Novvi.Domain.Entities;

/// <summary>Produto disponível para venda. N:N -&gt; Pedido.</summary>
public class Produto : EntidadeBase
{
    public string Nome { get; private set; }
    public string Descricao { get; private set; }
    public double Preco { get; private set; }

    public List<Pedido> Pedidos { get; private set; } = new();

    protected Produto() { }

    public Produto(string nome, string descricao, double preco)
    {
        if (string.IsNullOrWhiteSpace(nome) || nome.Length < 2)
            throw new DomainException("Nome do produto inválido");
        Nome = nome;

        if (string.IsNullOrWhiteSpace(descricao))
            throw new DomainException("Descrição do produto é obrigatória");
        Descricao = descricao;

        if (preco <= 0)
            throw new DomainException("Preço do produto inválido");
        Preco = preco;
    }

    /// <summary>Atualiza os dados cadastrais do produto.</summary>
    public void Atualizar(string nome, string descricao, double preco)
    {
        if (string.IsNullOrWhiteSpace(nome) || nome.Length < 2)
            throw new DomainException("Nome do produto inválido");
        Nome = nome;

        if (string.IsNullOrWhiteSpace(descricao))
            throw new DomainException("Descrição do produto é obrigatória");
        Descricao = descricao;

        if (preco <= 0)
            throw new DomainException("Preço do produto inválido");
        Preco = preco;
    }

    public override string ToString() => $"{Nome} (R$ {Preco}) - {Descricao}";
}
