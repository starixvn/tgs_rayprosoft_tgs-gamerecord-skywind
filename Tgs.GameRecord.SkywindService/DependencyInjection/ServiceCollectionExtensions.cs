using Tgs.GameRecord.SkywindService;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGameRecordEvolutionService(
        this IServiceCollection services,
        Action<RedisOptions, IServiceProvider> redisDBconfigure)
        => services
            .AddOptions<RedisOptions>()
            .Configure(redisDBconfigure)
            .Services
            .AddSingleton<RedisConnectionManager>();
}
