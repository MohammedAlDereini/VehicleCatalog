using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VehicleCatalog.Handler.Clients.Nhtsa;

namespace VehicleCatalog.Handler.DI;

/// <summary>Wires up the Handler layer: MediatR and the NHTSA HTTP client.</summary>
public static class DependenciesConfigurator
{
    private const string DefaultNhtsaBaseUrl = "https://vpic.nhtsa.dot.gov/api/";

    /// <summary>Registers all Handler-layer services.</summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <returns>The same <paramref name="services"/> instance, for chaining.</returns>
    public static IServiceCollection AddHandlerServices(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependenciesConfigurator).Assembly));

        var baseUrl = configuration["Nhtsa:BaseUrl"] ?? DefaultNhtsaBaseUrl;
        services.AddHttpClient<INhtsaVehicleClient, NhtsaVehicleClient>(client =>
        {
            client.BaseAddress = new Uri(baseUrl);
            client.Timeout = TimeSpan.FromSeconds(30);
        });

        return services;
    }
}
