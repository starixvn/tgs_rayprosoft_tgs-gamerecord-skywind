using System.Reflection;
using Tgs.GameCommand.Client;
using Tgs.GameInterface.SkywindService.Client;
using Tgs.GameRecord.SkywindService.Core;
using Tgs.GameRounds.Schedule.Client;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGameRecordCore(
        this IServiceCollection services,
        Action<RedisOptions, IServiceProvider> redisDBconfigure,
        Action<GameRecordInfoOptions, IServiceProvider> gameRecordInfoOptions,
        Action<GameCommandClientOptions, IServiceProvider> gameCommandClientOptions,
        Action<GameRoundsScheduleClientOptions, IServiceProvider> gameRoundsScheduleClientOptions,
        Action<GameInterfaceSkywindClientOptions, IServiceProvider> gameInterfaceSkywindClientOptions)
        => services
            .AddOptions<RedisOptions>()
            .Configure(redisDBconfigure)
            .Services
            .AddOptions<GameRecordInfoOptions>()
            .Configure(gameRecordInfoOptions)
            .Services
            .RegisterAllServiceUtilityTypes(
                Assembly.GetExecutingAssembly(),
                ServiceLifetime.Transient)
            .AddMemoryCache()
            .AddSingleton<RedisConnectionManager>()
            .AddGameCommandClient(gameCommandClientOptions)
            .AddGameRoundsScheduleClient(gameRoundsScheduleClientOptions)
            .AddGameInterfaceSkywindServiceClient(gameInterfaceSkywindClientOptions)
            .AddGameRoundsWorkClient()
            .AddSingleton<IScheduleFormDispatcher, ScheduleFormDispatcher>()
            .AddSingleton<IScheduleFormLock>(sp => ActivatorUtilities.CreateInstance<ScheduleFormLock>(sp, sp.GetRequiredService<RedisConnectionManager>().Database))
            .AddTransient<IUtensil>(sp => ActivatorUtilities.CreateInstance<UtensilRepository>(sp, TimeSpan.FromHours(1)));
}
