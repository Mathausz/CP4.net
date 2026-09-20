using Microsoft.AspNetCore.Mvc;
using Novvi.Application.DTO;
using Novvi.Application.Interfaces.Services;

namespace Novvi.Api.Controllers;

/// <summary>Cadastro e consulta de produtos.</summary>
[ApiController]
[Route("api/[controller]")]
public class ProdutosController(IProdutoService produtoService) : ControllerBase
{
    /// <summary>Lista todos os produtos ativos.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ProdutoResponse>), StatusCodes.Status200OK)]
    public IActionResult GetAll() => Ok(produtoService.GetAll());

    /// <summary>Busca um produto pelo id.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ProdutoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public IActionResult GetById(Guid id) => Ok(produtoService.GetById(id));

    /// <summary>Cadastra um novo produto.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ProdutoResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public IActionResult Post([FromBody] ProdutoRequest request)
    {
        var produto = produtoService.Create(request);
        return CreatedAtAction(nameof(GetById), new { id = produto.Id }, produto);
    }

    /// <summary>Atualiza um produto existente.</summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ProdutoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public IActionResult Put(Guid id, [FromBody] ProdutoRequest request) =>
        Ok(produtoService.Update(id, request));

    /// <summary>Inativa (soft delete) um produto.</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public IActionResult Delete(Guid id)
    {
        produtoService.Delete(id);
        return NoContent();
    }
}
