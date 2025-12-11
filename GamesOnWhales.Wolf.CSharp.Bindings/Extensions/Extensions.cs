namespace GamesOnWhales.Extensions;

using Microsoft.Extensions.DependencyInjection;

public static class Extensions
{
    public static IServiceCollection AddWolfApi(this IServiceCollection services)
    {
        services.AddSingleton<GamesOnWhales.WolfApi>()
            .AddHostedService(p => p.GetRequiredService<GamesOnWhales.WolfApi>());

        return services;
    }

    public static IServiceCollection AddWolfApi<T>(this IServiceCollection services) where T : GamesOnWhales.WolfApi
    {
        services.AddSingleton<T>()
            .AddHostedService(p => p.GetRequiredService<T>());
        return services;
    }

    public static IServiceCollection AddWolfApi<T>(this IServiceCollection services, Func<T> provider) where T : GamesOnWhales.WolfApi
    {
        services.AddSingleton(provider)
            .AddHostedService(p => p.GetRequiredService<T>());
        return services;
    }
}