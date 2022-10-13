namespace Tgs.GameRecord.SkywindService.Core;

public record RedisOptions
{
    public string ConnectionString { get; set; } = "localhost";
}
