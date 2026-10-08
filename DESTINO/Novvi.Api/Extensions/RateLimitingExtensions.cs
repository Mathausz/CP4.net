using System.Globalization;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Novvi.Api.Extensions;

/// <summary>Rate limit nativo (Microsoft.AspNetCore.RateLimiting), fixed window particionado por IP (CP5).</summary>
public static class RateLimitingExtensions
{
    /// <summary>POST /api/pedidos: 10 requisições por minuto por IP (estourável sem script).</summary>
    public const string PoliticaEscrita = "pedidos-escrita";

    /// <summary>GET /api/pedidos (v2): política mais branda, separada da de escrita.</summary>
    public const string PoliticaListagem = "listagem-v2";

    public const int LimiteEscrita = 10;
    public const int LimiteListagem = 60;
    public static readonly TimeSpan Janela = TimeSpan.FromMinutes(1);

    public static IServiceCollection AddNovviRateLimiting(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            options.AddPolicy(PoliticaEscrita, ctx => ParticaoPorIp(ctx, PoliticaEscrita, LimiteEscrita));
            options.AddPolicy(PoliticaListagem, ctx => ParticaoPorIp(ctx, PoliticaListagem, LimiteListagem));

            options.OnRejected = async (context, cancellationToken) =>
            {
                var http = context.HttpContext;

                var segundos = context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter)
                    ? (int)Math.Ceiling(retryAfter.TotalSeconds)
                    : (int)Janela.TotalSeconds;
                segundos = Math.Max(segundos, 1);

                http.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                http.Response.Headers.RetryAfter = segundos.ToString(CultureInfo.InvariantCulture);

                var problem = new ProblemDetails
                {
                    Type = "about:blank",
                    Title = "Muitas requisições",
                    Status = StatusCodes.Status429TooManyRequests,
                    Detail = $"Limite de requisições excedido. Tente novamente em {segundos} segundo(s).",
                    Instance = http.Request.Path
                };
                problem.Extensions["traceId"] = http.TraceIdentifier;
                problem.Extensions["retryAfterSeconds"] = segundos;

                await http.Response.WriteAsJsonAsync(
                    problem, options: null, contentType: "application/problem+json", cancellationToken);
            };
        });

        return services;
    }

    private static RateLimitPartition<string> ParticaoPorIp(HttpContext ctx, string politica, int limite)
    {
        var ip = ctx.Connection.RemoteIpAddress?.ToString() ?? "desconhecido";

        return RateLimitPartition.GetFixedWindowLimiter($"{politica}:{ip}", _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = limite,
            Window = Janela,
            QueueLimit = 0,
            AutoReplenishment = true
        });
    }
}
