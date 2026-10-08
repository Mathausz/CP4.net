using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Novvi.Api.Extensions;
using Novvi.Application.DTO;
using Novvi.Application.Interfaces.Services;

namespace Novvi.Api.Controllers;

/// <summary>
/// Criação e consulta de pedidos, incluindo o registro do pagamento.
/// Recurso versionado (CP5): v1.0 (deprecada, lista) e v2.0 (envelope paginado) compartilham o mesmo serviço.
/// </summary>
[ApiController]
[ApiVersion("1.0", Deprecated = true)]
[ApiVersion("2.0")]
[Route("api/[controller]")]
[Route("api/v{version:apiVersion}/[controller]")]
public class PedidosController(IPedidoService pedidoService, ILogger<PedidosController> logger) : ControllerBase
{
    /// <summary>[DEPRECADA - v1.0] Lista todos os pedidos ativos (contrato antigo: array, sem paginação).</summary>
    [HttpGet]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(IReadOnlyList<PedidoResponse>), StatusCodes.Status200OK)]
    public IActionResult GetAll() => Ok(pedidoService.GetAll());

    /// <summary>
    /// [v2.0] Lista pedidos ativos paginados. page &gt;= 1 (padrão 1); pageSize de 1 a 100 (padrão 20).
    /// Fora da faixa responde 400; página além do total responde 200 com items vazio.
    /// </summary>
    [HttpGet]
    [MapToApiVersion("2.0")]
    [EnableRateLimiting(RateLimitingExtensions.PoliticaListagem)]
    [ProducesResponseType(typeof(PagedResponse<PedidoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status429TooManyRequests)]
    public IActionResult GetPaged(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = Paginacao.PageSizePadrao) =>
        Ok(pedidoService.GetPaged(page, pageSize));

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
    [EnableRateLimiting(RateLimitingExtensions.PoliticaEscrita)]
    [ProducesResponseType(typeof(PedidoResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status429TooManyRequests)]
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
