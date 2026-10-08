using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Novvi.Application.Interfaces.Repositories;
using Novvi.Infrastructure.Persistence;
using Novvi.Infrastructure.Persistence.Repositories;

namespace Novvi.Infrastructure;

/// <summary>Registros de DI da camada de Infrastructure (DbContext + repositórios).</summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Novvi_Context_Oracle");

        services.AddDbContext<NovviContext>(options =>
            options.UseOracle(connectionString));

        // Repositório genérico (CP3): usado diretamente por Produto e Funcionario.
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        // Repositórios específicos: convivem com o genérico quando há consultas
        // adicionais (Include de agregados relacionados).
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<IProdutoRepository, ProdutoRepository>();
        services.AddScoped<IPedidoRepository, PedidoRepository>();

        return services;
    }
}
