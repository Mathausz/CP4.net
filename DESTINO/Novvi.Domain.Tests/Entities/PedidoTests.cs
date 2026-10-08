using Novvi.Domain.Entities;
using Novvi.Domain.Enums;
using Novvi.Domain.Exceptions;
using Xunit;

namespace Novvi.Domain.Tests.Entities;

public class PedidoTests
{
    private static Produto CriarProduto(double preco) => new("Produto teste", "Descrição teste", preco);

    [Fact]
    public void CalcularTotal_PedidoComProdutos_DeveSomarProdutosMaisFreteFixo()
    {
        // Arrange
        var produtos = new List<Produto> { CriarProduto(50.0), CriarProduto(30.0) };
        var pedido = new Pedido(Guid.NewGuid(), Guid.NewGuid(), produtos);

        // Act
        var total = pedido.CalcularTotal();

        // Assert
        Assert.Equal(50.0 + 30.0 + 9.99, total, precision: 2);
    }

    [Theory]
    [InlineData(true, false)]
    [InlineData(false, true)]
    public void Construtor_UsuarioOuFuncionarioVazio_DeveLancarDomainException(bool usuarioVazio, bool funcionarioVazio)
    {
        // Arrange
        var idUsuario = usuarioVazio ? Guid.Empty : Guid.NewGuid();
        var idFuncionario = funcionarioVazio ? Guid.Empty : Guid.NewGuid();
        var produtos = new List<Produto> { CriarProduto(10.0) };

        // Act & Assert
        Assert.Throws<DomainException>(() => new Pedido(idUsuario, idFuncionario, produtos));
    }

    [Fact]
    public void Construtor_SemProdutos_DeveLancarDomainException()
    {
        // Arrange
        var produtosVazio = new List<Produto>();

        // Act & Assert
        Assert.Throws<DomainException>(() => new Pedido(Guid.NewGuid(), Guid.NewGuid(), produtosVazio));
    }

    [Fact]
    public void RegistrarPagamento_PedidoSemPagamento_DeveAssociarPagamento()
    {
        // Arrange
        var pedido = new Pedido(Guid.NewGuid(), Guid.NewGuid(), new List<Produto> { CriarProduto(10.0) });
        var pagamento = new Pagamento(TipoPagamento.Pix, pedido.Id);

        // Act
        pedido.RegistrarPagamento(pagamento);

        // Assert
        Assert.Equal(pagamento, pedido.Pagamento);
    }

    [Fact]
    public void RegistrarPagamento_PedidoJaComPagamento_DeveLancarDomainException()
    {
        // Arrange
        var pedido = new Pedido(Guid.NewGuid(), Guid.NewGuid(), new List<Produto> { CriarProduto(10.0) });
        pedido.RegistrarPagamento(new Pagamento(TipoPagamento.Pix, pedido.Id));

        // Act & Assert
        Assert.Throws<DomainException>(() => pedido.RegistrarPagamento(new Pagamento(TipoPagamento.Cartao, pedido.Id)));
    }
}
