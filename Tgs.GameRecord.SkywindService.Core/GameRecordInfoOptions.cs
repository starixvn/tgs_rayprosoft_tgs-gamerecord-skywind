namespace Tgs.GameRecord.SkywindService.Core;

public record GameRecordInfoOptions()
{
    public string GameProviderCode { get; set; } = default!;
    public double IntervalMinutes { get; set; } = 10d;
    public double IntervalMinutesMax { get; set; } = 60d;
    public double IntervalMinutesMinimum { get; set; } = 6d;
    public double LockMinute { get; set; } = 4d;
}
