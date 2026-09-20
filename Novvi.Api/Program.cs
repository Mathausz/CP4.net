using System.Reflection;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi;
using Novvi.Api.Exceptions;
using Novvi.Api.Extensions;
using Novvi.Api.Health;
using Novvi.Infrastructure;
using Novvi.Infrastructure.Persistence;

namespace Novvi.Api;

/// <summary>Ponto de entrada da aplicação. Configura DI, Swagger, health checks e o pipeline HTTP.</summary>
public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Camadas de aplicação e infraestrutura (CP2/CP3)
        builder.Services.AddControllers();
        builder.Services.AddApplicationServices();
        builder.Services.AddInfrastructure(builder.Configuration);

        // Tratamento global de erros (CP3)
        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
        builder.Services.AddProblemDetails();

        // Swagger / OpenAPI completo (CP3)
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Novvi API",
                Version = "v1",
                Description = "API REST do domínio Novvi (usuários, funcionários, produtos e pedidos) - " +
                               "FIAP Challenge, Clean Architecture."
            });

            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
                options.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
        });

        // Health checks (CP4): processo (self) + banco de dados
        builder.Services
            .AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy("API no ar"), tags: ["alive"])
            .AddDbContextCheck<NovviContext>(name: "database", tags: ["ready"]);

        var app = builder.Build();

        app.UseExceptionHandler();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Novvi API v1");
                options.RoutePrefix = "swagger";
            });
        }

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();

        // GET /health único endpoint, com todos os checks (self + database)
        app.MapHealthChecks("/health", new HealthCheckOptions
        {
            ResponseWriter = HealthCheckResponseWriter.WriteJsonResponse
        });

        app.Run();
    }
}
