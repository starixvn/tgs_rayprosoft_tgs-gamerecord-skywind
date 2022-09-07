namespace Tgs.GameRecord.SkywindService.Core;

public record GameRecordInfoOptions()
{
    public string GameProviderCode { get; set; } = default!;
    public double MinimumIntervalMinute { get; set; } = 6d;
    public double IntervalMinute { get; set; } = 10d;
    public double LockMinute { get; set; } = 8.5d;
}
