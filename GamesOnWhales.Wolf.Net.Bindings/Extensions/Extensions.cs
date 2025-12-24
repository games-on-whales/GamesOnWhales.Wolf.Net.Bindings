using Microsoft.Extensions.Hosting;

namespace GamesOnWhales.Extensions;

using Microsoft.Extensions.DependencyInjection;

public static class Extensions
{
    public static IServiceCollection AddWolfApi(this IServiceCollection services)
    {
        services.AddSingleton<WolfApi>()
            .AddHostedService(p => p.GetRequiredService<WolfApi>());

        return services;
    }

    public static IServiceCollection AddWolfApi<T>(this IServiceCollection services)
        where T : class, IWolfApi
    {
        services.AddSingleton<T>()
            .AddHostedService(p => p.GetRequiredService<T>());
        return services;
    }

    public static IServiceCollection AddWolfApi<T>(this IServiceCollection services, Func<T> provider) 
        where T : class, IWolfApi
    {
        services.AddSingleton(provider)
            .AddHostedService(p => p.GetRequiredService<T>());
        return services;
    }
}