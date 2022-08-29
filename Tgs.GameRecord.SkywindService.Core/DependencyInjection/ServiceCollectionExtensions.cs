using System.Reflection;
using StackExchange.Redis;
using Tgs.GameInterface.SkywindService.Client;
using Tgs.GameProviders.QueryServer.Client;
using Tgs.GameRecord.SkywindService.Core;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGameRecordEvolutionServiceCore(
        this IServiceCollection services,
        Action<GameRecordInfoOptions, IServiceProvider> gameRecordInfoOptions,
        Action<GameInterfaceSkywindClientOptions, IServiceProvider> gameInterfaceSkywindClientOptions,
        Action<GameProvidersQueryServerClientOptions, IServiceProvider> gameProvidersQueryServerClientOptions,
        Func<IServiceProvider, IDatabase> redisDatabaseResolver
        )
        => services
            .AddOptions<GameRecordInfoOptions>()
            .Configure(gameRecordInfoOptions)
            .Services
            .RegisterAllServiceUtilityTypes(
                Assembly.GetExecutingAssembly(),
                ServiceLifetime.Transient)
            .AddGameInterfaceSkywindServiceClient(gameInterfaceSkywindClientOptions)
            .AddGameProvidersQueryServerClient(gameProvidersQueryServerClientOptions)
            .AddTransient<IScheduleDispatch>(sp => ActivatorUtilities.CreateInstance<ScheduleDispatch>(sp, redisDatabaseResolver(sp)))
            .AddTransient<IUtensil, UtensilRepository>()
            .Decorate<IUtensil>((innerRepository, sp) => ActivatorUtilities.CreateInstance<UtensilMemoryCacheRepository>(sp, innerRepository, TimeSpan.FromHours(1)));
}
