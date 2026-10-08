using System.Reflection;
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;
using Novvi.Api.Exceptions;
using Novvi.Api.Extensions;
using Novvi.Api.Health;
using Novvi.Api.Swagger;
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

        // Versionamento de API (CP5): padrão 2.0, omissão cai na 2.0, headers api-supported/deprecated-versions
        builder.Services
            .AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(2, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
                options.ApiVersionReader = ApiVersionReader.Combine(
                    new QueryStringApiVersionReader("api-version"),
                    new HeaderApiVersionReader("X-Api-Version"),
                    new UrlSegmentApiVersionReader());
            })
            .AddMvc()
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVVV";
                options.SubstituteApiVersionInUrl = true;
            });

        // Swagger / OpenAPI: um documento por versão (CP3 + CP5)
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
        builder.Services.AddSwaggerGen(options =>
        {
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
                options.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
        });

        // Rate limit nativo, fixed window por IP (CP5)
        builder.Services.AddNovviRateLimiting();

        // Health checks (CP4): processo (self) + banco de dados
        builder.Services
            .AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy("API no ar"), tags: ["alive"])
            .AddDbContextCheck<NovviContext>(name: "database", tags: ["ready"]);

        var app = builder.Build();

        app.UseExceptionHandler();

        // Depois do UseExceptionHandler e antes do MapControllers (CP5)
        app.UseRateLimiter();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
                foreach (var description in provider.ApiVersionDescriptions.Reverse())
                {
                    var nome = description.IsDeprecated
                        ? $"Novvi API {description.GroupName} (DEPRECADA)"
                        : $"Novvi API {description.GroupName}";
                    options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", nome);
                }

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
        }).DisableRateLimiting(); // /health nunca divide o teto (CP5)

        app.Run();
    }
}
