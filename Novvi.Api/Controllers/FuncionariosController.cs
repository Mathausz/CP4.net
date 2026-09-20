using Microsoft.AspNetCore.Mvc;
using Novvi.Application.DTO;
using Novvi.Application.Interfaces.Services;

namespace Novvi.Api.Controllers;

/// <summary>Cadastro e consulta de funcionários.</summary>
[ApiController]
[Route("api/[controller]")]
public class FuncionariosController(IFuncionarioService funcionarioService) : ControllerBase
{
    /// <summary>Lista todos os funcionários ativos.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<FuncionarioResponse>), StatusCodes.Status200OK)]
    public IActionResult GetAll() => Ok(funcionarioService.GetAll());

    /// <summary>Busca um funcionário pelo id.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(FuncionarioResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public IActionResult GetById(Guid id) => Ok(funcionarioService.GetById(id));

    /// <summary>Cadastra um novo funcionário.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(FuncionarioResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public IActionResult Post([FromBody] FuncionarioRequest request)
    {
        var funcionario = funcionarioService.Create(request);
        return CreatedAtAction(nameof(GetById), new { id = funcionario.Id }, funcionario);
    }

    /// <summary>Atualiza um funcionário existente.</summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(FuncionarioResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public IActionResult Put(Guid id, [FromBody] FuncionarioRequest request) =>
        Ok(funcionarioService.Update(id, request));

    /// <summary>Inativa (soft delete) um funcionário.</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public IActionResult Delete(Guid id)
    {
        funcionarioService.Delete(id);
        return NoContent();
    }
}
