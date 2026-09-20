using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Novvi.Domain.Exceptions;

namespace Novvi.Api.Exceptions;

/// <summary>
/// Trata centralmente as exceções não capturadas pelos controllers (CP3),
/// convertendo-as em respostas ProblemDetails (RFC 7807).
/// </summary>
public sealed class GlobalExceptionHandler(
    ILogger<GlobalExceptionHandler> logger,
    IHostEnvironment environment) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var traceId = httpContext.TraceIdentifier;

        logger.LogError(exception, "Exceção não tratada: {Message}. TraceId: {TraceId}", exception.Message, traceId);

        var (statusCode, title, detail) = MapException(exception);

        httpContext.Response.StatusCode = statusCode;
        httpContext.Response.ContentType = "application/problem+json";

        var problem = new ProblemDetails
        {
            Type = "about:blank",
            Title = title,
            Status = statusCode,
            Detail = environment.IsDevelopment() ? detail : "Ocorreu um erro ao processar a requisição.",
            Instance = httpContext.Request.Path
        };

        if (environment.IsDevelopment())
            problem.Extensions["traceId"] = traceId;

        await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);
        return true;
    }

    private static (int StatusCode, string Title, string Detail) MapException(Exception exception) =>
        exception switch
        {
            NotFoundException e => (StatusCodes.Status404NotFound, "Recurso não encontrado", e.Message),
            DomainException e => (StatusCodes.Status400BadRequest, "Não foi possível concluir a operação", e.Message),
            ArgumentNullException e => (StatusCodes.Status400BadRequest, "Requisição inválida", e.Message),
            ArgumentException e => (StatusCodes.Status400BadRequest, "Requisição inválida", e.Message),
            KeyNotFoundException e => (StatusCodes.Status404NotFound, "Recurso não encontrado", e.Message),
            InvalidOperationException e => (StatusCodes.Status409Conflict, "Conflito ao concluir a operação", e.Message),
            _ => (StatusCodes.Status500InternalServerError, "Erro interno do servidor",
                "Ocorreu um erro inesperado. Tente novamente mais tarde.")
        };
}
