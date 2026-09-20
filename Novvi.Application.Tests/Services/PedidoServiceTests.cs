using Microsoft.Extensions.Logging;
using Moq;
using Novvi.Application.DTO;
using Novvi.Application.Interfaces.Repositories;
using Novvi.Application.Services;
using Novvi.Domain.Entities;
using Novvi.Domain.Exceptions;
using Xunit;

namespace Novvi.Application.Tests.Services;

public class PedidoServiceTests
{
    private static PedidoService CriarService(
        Mock<IPedidoRepository> pedidoRepositoryMock,
        Mock<IProdutoRepository> produtoRepositoryMock,
        Mock<IRepository<Usuario>> usuarioRepositoryMock,
        Mock<IRepository<Funcionario>> funcionarioRepositoryMock) =>
        new(
            pedidoRepositoryMock.Object,
            produtoRepositoryMock.Object,
            usuarioRepositoryMock.Object,
            funcionarioRepositoryMock.Object,
            Mock.Of<ILogger<PedidoService>>());

    [Fact]
    public void Create_UsuarioInexistente_DeveLancarNotFoundException_NaoDevePersistirPedido()
    {
        // Arrange
        var pedidoRepositoryMock = new Mock<IPedidoRepository>();
        var produtoRepositoryMock = new Mock<IProdutoRepository>();
        var usuarioRepositoryMock = new Mock<IRepository<Usuario>>();
        var funcionarioRepositoryMock = new Mock<IRepository<Funcionario>>();

        usuarioRepositoryMock.Setup(r => r.ExistsById(It.IsAny<Guid>())).Returns(false);

        var service = CriarService(pedidoRepositoryMock, produtoRepositoryMock, usuarioRepositoryMock, funcionarioRepositoryMock);
        var request = new PedidoRequest
        {
            IdUsuario = Guid.NewGuid(),
            IdFuncionario = Guid.NewGuid(),
            ProdutosIds = [Guid.NewGuid()]
        };

        // Act & Assert
        Assert.Throws<NotFoundException>(() => service.Create(request));
        pedidoRepositoryMock.Verify(r => r.Add(It.IsAny<Pedido>()), Times.Never);
    }

    [Fact]
    public void Create_FuncionarioInexistente_DeveLancarNotFoundException_NaoDevePersistirPedido()
    {
        // Arrange
        var pedidoRepositoryMock = new Mock<IPedidoRepository>();
        var produtoRepositoryMock = new Mock<IProdutoRepository>();
        var usuarioRepositoryMock = new Mock<IRepository<Usuario>>();
        var funcionarioRepositoryMock = new Mock<IRepository<Funcionario>>();

        usuarioRepositoryMock.Setup(r => r.ExistsById(It.IsAny<Guid>())).Returns(true);
        funcionarioRepositoryMock.Setup(r => r.ExistsById(It.IsAny<Guid>())).Returns(false);

        var service = CriarService(pedidoRepositoryMock, produtoRepositoryMock, usuarioRepositoryMock, funcionarioRepositoryMock);
        var request = new PedidoRequest
        {
            IdUsuario = Guid.NewGuid(),
            IdFuncionario = Guid.NewGuid(),
            ProdutosIds = [Guid.NewGuid()]
        };

        // Act & Assert
        Assert.Throws<NotFoundException>(() => service.Create(request));
        pedidoRepositoryMock.Verify(r => r.Add(It.IsAny<Pedido>()), Times.Never);
    }

    [Fact]
    public void Create_PedidoValido_DevePersistirUmaVez()
    {
        // Arrange
        var pedidoRepositoryMock = new Mock<IPedidoRepository>();
        var produtoRepositoryMock = new Mock<IProdutoRepository>();
        var usuarioRepositoryMock = new Mock<IRepository<Usuario>>();
        var funcionarioRepositoryMock = new Mock<IRepository<Funcionario>>();

        var produto = new Produto("Produto teste", "Descrição teste", 10.0);

        usuarioRepositoryMock.Setup(r => r.ExistsById(It.IsAny<Guid>())).Returns(true);
        funcionarioRepositoryMock.Setup(r => r.ExistsById(It.IsAny<Guid>())).Returns(true);
        produtoRepositoryMock.Setup(r => r.GetByIds(It.IsAny<IEnumerable<Guid>>())).Returns([produto]);
        pedidoRepositoryMock.Setup(r => r.Add(It.IsAny<Pedido>())).Returns((Pedido p) => p);

        var service = CriarService(pedidoRepositoryMock, produtoRepositoryMock, usuarioRepositoryMock, funcionarioRepositoryMock);
        var request = new PedidoRequest
        {
            IdUsuario = Guid.NewGuid(),
            IdFuncionario = Guid.NewGuid(),
            ProdutosIds = [produto.Id]
        };

        // Act
        var response = service.Create(request);

        // Assert
        Assert.Single(response.Produtos);
        pedidoRepositoryMock.Verify(r => r.Add(It.IsAny<Pedido>()), Times.Once);
    }
}
