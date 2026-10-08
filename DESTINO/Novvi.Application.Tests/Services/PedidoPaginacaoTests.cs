using Microsoft.Extensions.Logging;
using Moq;
using Novvi.Application.DTO;
using Novvi.Application.Exceptions;
using Novvi.Application.Interfaces.Repositories;
using Novvi.Application.Services;
using Novvi.Domain.Entities;
using Xunit;

namespace Novvi.Application.Tests.Services;

/// <summary>Regra de page / pageSize da listagem v2 (CP5). Sem subir API nem banco.</summary>
public class PedidoPaginacaoTests
{
    private static PedidoService CriarService(Mock<IPedidoRepository> pedidoRepositoryMock) =>
        new(
            pedidoRepositoryMock.Object,
            Mock.Of<IProdutoRepository>(),
            Mock.Of<IRepository<Usuario>>(),
            Mock.Of<IRepository<Funcionario>>(),
            Mock.Of<ILogger<PedidoService>>());

    [Theory]
    [InlineData(0, 20)]
    [InlineData(-1, 20)]
    [InlineData(1, 0)]
    [InlineData(1, -5)]
    [InlineData(1, 101)]
    [InlineData(1, 9999)]
    public void GetPaged_PageOuPageSizeInvalido_DeveLancarPaginacaoInvalida_NaoDeveConsultarRepositorio(int page, int pageSize)
    {
        // Arrange
        var repositoryMock = new Mock<IPedidoRepository>();
        var service = CriarService(repositoryMock);

        // Act & Assert
        Assert.Throws<PaginacaoInvalidaException>(() => service.GetPaged(page, pageSize));
        repositoryMock.Verify(r => r.GetPagedCompleto(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
    }

    [Theory]
    [InlineData(1, 1)]
    [InlineData(1, 20)]
    [InlineData(3, 100)]
    public void GetPaged_IntervaloValido_DeveConsultarRepositorioUmaVez(int page, int pageSize)
    {
        // Arrange
        var repositoryMock = new Mock<IPedidoRepository>();
        repositoryMock
            .Setup(r => r.GetPagedCompleto(page, pageSize))
            .Returns((Array.Empty<Pedido>(), 0));
        var service = CriarService(repositoryMock);

        // Act
        var resultado = service.GetPaged(page, pageSize);

        // Assert
        Assert.Equal(page, resultado.Page);
        Assert.Equal(pageSize, resultado.PageSize);
        Assert.Empty(resultado.Items);
        repositoryMock.Verify(r => r.GetPagedCompleto(page, pageSize), Times.Once);
    }

    [Fact]
    public void GetPaged_TotalNaoDivisivel_TotalPagesDeveSerOTetoDaDivisao()
    {
        // Arrange: 137 itens com pageSize 20 => 7 páginas
        var repositoryMock = new Mock<IPedidoRepository>();
        repositoryMock
            .Setup(r => r.GetPagedCompleto(1, 20))
            .Returns((Array.Empty<Pedido>(), 137));
        var service = CriarService(repositoryMock);

        // Act
        var resultado = service.GetPaged(1, 20);

        // Assert
        Assert.Equal(137, resultado.TotalItems);
        Assert.Equal(7, resultado.TotalPages);
        Assert.False(resultado.HasPrevious);
        Assert.True(resultado.HasNext);
    }
}
