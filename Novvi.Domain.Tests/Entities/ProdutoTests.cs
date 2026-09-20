using Novvi.Domain.Entities;
using Novvi.Domain.Exceptions;
using Xunit;

namespace Novvi.Domain.Tests.Entities;

public class ProdutoTests
{
    [Fact]
    public void Construtor_DadosValidos_DeveCriarProdutoAtivo()
    {
        // Arrange
        const string nome = "Camiseta Novvi";
        const string descricao = "Camiseta 100% algodão";
        const double preco = 79.90;

        // Act
        var produto = new Produto(nome, descricao, preco);

        // Assert
        Assert.Equal(nome, produto.Nome);
        Assert.Equal(descricao, produto.Descricao);
        Assert.Equal(preco, produto.Preco);
        Assert.True(produto.Ativo);
        Assert.NotEqual(Guid.Empty, produto.Id);
    }

    [Theory]
    [InlineData("", "Descrição válida", 10.0)]
    [InlineData("A", "Descrição válida", 10.0)]
    [InlineData("Nome válido", "", 10.0)]
    [InlineData("Nome válido", "Descrição válida", 0)]
    [InlineData("Nome válido", "Descrição válida", -5)]
    public void Construtor_DadosInvalidos_DeveLancarDomainException(string nome, string descricao, double preco)
    {
        // Act & Assert
        Assert.Throws<DomainException>(() => new Produto(nome, descricao, preco));
    }

    [Fact]
    public void Atualizar_DadosValidos_DeveAlterarPropriedades()
    {
        // Arrange
        var produto = new Produto("Nome antigo", "Descrição antiga", 10.0);

        // Act
        produto.Atualizar("Nome novo", "Descrição nova", 20.0);

        // Assert
        Assert.Equal("Nome novo", produto.Nome);
        Assert.Equal("Descrição nova", produto.Descricao);
        Assert.Equal(20.0, produto.Preco);
    }

    [Fact]
    public void Desativar_ProdutoAtivo_DeveMarcarComoInativo()
    {
        // Arrange
        var produto = new Produto("Nome", "Descrição", 10.0);

        // Act
        produto.Desativar();

        // Assert
        Assert.False(produto.Ativo);
    }
}
