namespace Tgs.GameRecord.SkywindService;

public record RedisOptions
{
    public string ConnectionString { get; set; } = "localhost";
}
