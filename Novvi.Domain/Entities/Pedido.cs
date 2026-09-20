using Novvi.Domain.Commons;
using Novvi.Domain.Exceptions;

namespace Novvi.Domain.Entities;

/// <summary>
/// Pedido realizado por um usuário e atendido por um funcionário.
/// N:1 -&gt; Usuario | N:1 -&gt; Funcionario | N:N -&gt; Produto | 1:1 -&gt; Pagamento
/// </summary>
public class Pedido : EntidadeBase
{
    private const double FreteFixo = 9.99;

    public DateTime Data { get; private set; } = DateTime.Now;
    public double Frete { get; private set; } = FreteFixo;

    public Guid IdUsuario { get; private set; }
    public Guid IdFuncionario { get; private set; }

    public List<Produto> Produtos { get; private set; } = new();

    /// <summary>Pagamento associado a este pedido (registrado após a criação).</summary>
    public Pagamento? Pagamento { get; private set; }

    protected Pedido() { }

    public Pedido(Guid idUsuario, Guid idFuncionario, List<Produto> produtos)
    {
        if (idUsuario == Guid.Empty)
            throw new DomainException("Pedido precisa estar associado a um usuário");
        IdUsuario = idUsuario;

        if (idFuncionario == Guid.Empty)
            throw new DomainException("Pedido precisa estar associado a um funcionário");
        IdFuncionario = idFuncionario;

        if (produtos is null || produtos.Count == 0)
            throw new DomainException("Pedido precisa ter ao menos um produto");
        Produtos = produtos;
    }

    /// <summary>Calcula o valor total do pedido (produtos + frete).</summary>
    public double CalcularTotal() => Produtos.Sum(p => p.Preco) + Frete;

    /// <summary>Associa o pagamento a este pedido. Só pode ser feito uma vez.</summary>
    public void RegistrarPagamento(Pagamento pagamento)
    {
        if (Pagamento is not null)
            throw new DomainException("Este pedido já possui um pagamento registrado");

        if (pagamento.IdPedido != Id)
            throw new DomainException("Pagamento não pertence a este pedido");

        Pagamento = pagamento;
    }

    public string ListarProdutos() => string.Join(", ", Produtos.Select(p => p.Nome));

    public override string ToString() =>
        $"{Data:dd/MM/yyyy} (frete: {Frete}) - Produtos: {ListarProdutos()}";
}
