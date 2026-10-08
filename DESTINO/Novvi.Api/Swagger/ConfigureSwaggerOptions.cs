using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Novvi.Api.Swagger;

/// <summary>Cria um documento Swagger por versão da API (CP5). A v1 é marcada como deprecada na descrição.</summary>
public sealed class ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider)
    : IConfigureOptions<SwaggerGenOptions>
{
    public void Configure(SwaggerGenOptions options)
    {
        foreach (var description in provider.ApiVersionDescriptions)
        {
            var texto = "API REST do domínio Novvi (usuários, funcionários, produtos e pedidos) - " +
                        "FIAP Challenge, Clean Architecture.";

            if (description.IsDeprecated)
                texto += " ⚠️ ESTA VERSÃO ESTÁ DEPRECADA (v1.0): a listagem de pedidos devolve a lista inteira, " +
                         "sem paginação. Migre para a v2.0 (envelope paginado). A v1 continua no ar, apenas avisa.";

            options.SwaggerDoc(description.GroupName, new OpenApiInfo
            {
                Title = "Novvi API",
                Version = description.ApiVersion.ToString(),
                Description = texto
            });
        }
    }
}
