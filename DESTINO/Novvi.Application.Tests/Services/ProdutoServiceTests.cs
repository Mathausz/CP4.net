using Moq;
using Novvi.Application.DTO;
using Novvi.Application.Interfaces.Repositories;
using Novvi.Application.Services;
using Novvi.Domain.Entities;
using Novvi.Domain.Exceptions;
using Xunit;

namespace Novvi.Application.Tests.Services;

public class ProdutoServiceTests
{
    [Fact]
    public void GetById_ProdutoInexistente_DeveLancarNotFoundException()
    {
        // Arrange
        var repositoryMock = new Mock<IRepository<Produto>>();
        repositoryMock.Setup(r => r.GetById(It.IsAny<Guid>())).Returns((Produto?)null);
        var service = new ProdutoService(repositoryMock.Object);

        // Act & Assert
        Assert.Throws<NotFoundException>(() => service.GetById(Guid.NewGuid()));
    }

    [Fact]
    public void Create_ProdutoValido_DevePersistirUmaVez()
    {
        // Arrange
        var repositoryMock = new Mock<IRepository<Produto>>();
        repositoryMock.Setup(r => r.Add(It.IsAny<Produto>())).Returns((Produto p) => p);
        var service = new ProdutoService(repositoryMock.Object);
        var request = new ProdutoRequest { Nome = "Produto teste", Descricao = "Descrição teste", Preco = 25.0 };

        // Act
        var response = service.Create(request);

        // Assert
        Assert.Equal(request.Nome, response.Nome);
        repositoryMock.Verify(r => r.Add(It.IsAny<Produto>()), Times.Once);
    }

    [Fact]
    public void Delete_ProdutoInexistente_DeveLancarNotFoundException()
    {
        // Arrange
        var repositoryMock = new Mock<IRepository<Produto>>();
        repositoryMock.Setup(r => r.Delete(It.IsAny<Guid>())).Returns(false);
        var service = new ProdutoService(repositoryMock.Object);

        // Act & Assert
        Assert.Throws<NotFoundException>(() => service.Delete(Guid.NewGuid()));
    }
}
