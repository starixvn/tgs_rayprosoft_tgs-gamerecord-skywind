namespace Tgs.GameRecord.Skywind.Core;

public record RedisOptions
{
    public string ConnectionString { get; set; } = "localhost";
}
