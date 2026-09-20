using Microsoft.AspNetCore.Mvc;
using Novvi.Application.DTO;
using Novvi.Application.Interfaces.Services;

namespace Novvi.Api.Controllers;

/// <summary>Criação e consulta de pedidos, incluindo o registro do pagamento.</summary>
[ApiController]
[Route("api/[controller]")]
public class PedidosController(IPedidoService pedidoService, ILogger<PedidosController> logger) : ControllerBase
{
    /// <summary>Lista todos os pedidos ativos, com produtos e pagamento.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<PedidoResponse>), StatusCodes.Status200OK)]
    public IActionResult GetAll() => Ok(pedidoService.GetAll());

    /// <summary>Busca um pedido pelo id.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(PedidoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public IActionResult GetById(Guid id) => Ok(pedidoService.GetById(id));

    /// <summary>
    /// Cria um novo pedido para um usuário, atendido por um funcionário, com uma lista de produtos.
    /// Fluxo de escrita instrumentado com log estruturado + traceId (CP4).
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(PedidoResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public IActionResult Post([FromBody] PedidoRequest request)
    {
        var traceId = HttpContext.TraceIdentifier;

        logger.LogInformation(
            "Recebida requisição de criação de pedido. TraceId: {TraceId}, IdUsuario: {IdUsuario}",
            traceId, request.IdUsuario);

        var pedido = pedidoService.Create(request);

        logger.LogInformation(
            "Requisição de criação de pedido concluída com sucesso. TraceId: {TraceId}, IdPedido: {IdPedido}",
            traceId, pedido.Id);

        return CreatedAtAction(nameof(GetById), new { id = pedido.Id }, pedido);
    }

    /// <summary>Registra o pagamento de um pedido já existente.</summary>
    [HttpPost("{id:guid}/pagamento")]
    [ProducesResponseType(typeof(PedidoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public IActionResult PostPagamento(Guid id, [FromBody] PagamentoRequest request) =>
        Ok(pedidoService.RegistrarPagamento(id, request));

    /// <summary>Inativa (soft delete) um pedido.</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public IActionResult Delete(Guid id)
    {
        pedidoService.Delete(id);
        return NoContent();
    }
}
