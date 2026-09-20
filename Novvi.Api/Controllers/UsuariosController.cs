using Microsoft.AspNetCore.Mvc;
using Novvi.Application.DTO;
using Novvi.Application.Interfaces.Services;

namespace Novvi.Api.Controllers;

/// <summary>Cadastro e consulta de usuários (clientes) e seus endereços.</summary>
[ApiController]
[Route("api/[controller]")]
public class UsuariosController(IUsuarioService usuarioService) : ControllerBase
{
    /// <summary>Lista todos os usuários ativos, com seus endereços.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<UsuarioResponse>), StatusCodes.Status200OK)]
    public IActionResult GetAll() => Ok(usuarioService.GetAll());

    /// <summary>Busca um usuário pelo id.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(UsuarioResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public IActionResult GetById(Guid id) => Ok(usuarioService.GetById(id));

    /// <summary>Cadastra um novo usuário, opcionalmente já com endereços.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(UsuarioResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public IActionResult Post([FromBody] UsuarioRequest request)
    {
        var usuario = usuarioService.Create(request);
        return CreatedAtAction(nameof(GetById), new { id = usuario.Id }, usuario);
    }

    /// <summary>Atualiza os dados cadastrais de um usuário.</summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(UsuarioResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public IActionResult Put(Guid id, [FromBody] UsuarioRequest request) =>
        Ok(usuarioService.Update(id, request));

    /// <summary>Adiciona um novo endereço a um usuário existente.</summary>
    [HttpPost("{id:guid}/enderecos")]
    [ProducesResponseType(typeof(UsuarioResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public IActionResult PostEndereco(Guid id, [FromBody] EnderecoRequest request) =>
        Ok(usuarioService.AdicionarEndereco(id, request));

    /// <summary>Inativa (soft delete) um usuário.</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public IActionResult Delete(Guid id)
    {
        usuarioService.Delete(id);
        return NoContent();
    }
}
