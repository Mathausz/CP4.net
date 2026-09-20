using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Novvi.Api.Health;

/// <summary>Formata o relatório de health checks como JSON legível (CP4).</summary>
public static class HealthCheckResponseWriter
{
    public static Task WriteJsonResponse(HttpContext context, HealthReport report)
    {
        context.Response.ContentType = "application/json";

        var payload = new
        {
            status = report.Status.ToString(),
            duration = report.TotalDuration.TotalMilliseconds,
            checks = report.Entries.Select(e => new
            {
                name = e.Key,
                status = e.Value.Status.ToString(),
                description = e.Value.Description,
                durationMs = e.Value.Duration.TotalMilliseconds,
                error = context.RequestServices
                    .GetRequiredService<IHostEnvironment>()
                    .IsDevelopment()
                    ? e.Value.Exception?.Message
                    : null
            })
        };

        return context.Response.WriteAsync(JsonSerializer.Serialize(payload));
    }
}
