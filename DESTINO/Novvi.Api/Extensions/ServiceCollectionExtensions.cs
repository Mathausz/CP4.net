using Novvi.Application.Interfaces.Services;
using Novvi.Application.Services;

namespace Novvi.Api.Extensions;

/// <summary>Registros de DI dos serviços de aplicação (CP3).</summary>
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IUsuarioService, UsuarioService>();
        services.AddScoped<IProdutoService, ProdutoService>();
        services.AddScoped<IFuncionarioService, FuncionarioService>();
        services.AddScoped<IPedidoService, PedidoService>();
        return services;
    }
}
