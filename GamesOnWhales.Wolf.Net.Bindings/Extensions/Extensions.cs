using Microsoft.Extensions.Hosting;

namespace GamesOnWhales.Extensions;

using Microsoft.Extensions.DependencyInjection;

public static partial class Extensions
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

    public static IServiceCollection AddAllEventHandler(this IServiceCollection services)
    {
        var types = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(p => typeof(SSE.ISseEventHandler).IsAssignableFrom(p) && p.IsClass);

        foreach (var type in types)
        {
            services.Add(new ServiceDescriptor(typeof(SSE.ISseEventHandler), type, ServiceLifetime.Transient));
        }
        
        return services;
    }
}