using System.Reflection;
using Tgs.GameInterface.SkywindService.Client;
using Tgs.GameProviders.QueryServer.Client;
using Tgs.GameRecord.SkywindService.Core;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGameRecordCore(
        this IServiceCollection services,
        Action<RedisOptions, IServiceProvider> redisDBconfigure,
        Action<GameRecordInfoOptions, IServiceProvider> gameRecordInfoOptions,
        Action<GameInterfaceSkywindClientOptions, IServiceProvider> gameInterfaceSkywindClientOptions,
        Action<GameProvidersQueryServerClientOptions, IServiceProvider> gameProvidersQueryServerClientOptions)
        => services
            .AddOptions<RedisOptions>()
            .Configure(redisDBconfigure)
            .Services
            .AddOptions<GameRecordInfoOptions>()
            .Configure(gameRecordInfoOptions)
            .Services
            .AddMemoryCache()
            .RegisterAllServiceUtilityTypes(
                Assembly.GetExecutingAssembly(),
                ServiceLifetime.Transient)
            .AddGameInterfaceSkywindServiceClient(gameInterfaceSkywindClientOptions)
            .AddGameProvidersQueryServerClient(gameProvidersQueryServerClientOptions)
            .AddSingleton<RedisConnectionManager>()
            .AddTransient<IScheduleDispatch>(sp => ActivatorUtilities.CreateInstance<ScheduleDispatch>(sp, sp.GetRequiredService<RedisConnectionManager>().Database))
            .AddTransient<IUtensil, UtensilRepository>()
            .Decorate<IUtensil>((innerRepository, sp) => ActivatorUtilities.CreateInstance<UtensilMemoryCacheRepository>(sp, innerRepository, TimeSpan.FromHours(1)));
}
