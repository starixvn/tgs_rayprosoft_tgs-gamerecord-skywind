namespace Tgs.GameRecord.Skywind.Core;

public record GameRecordInfoOptions()
{
    public string GameProviderCode { get; set; } = default!;
    public string GameProviderConfiguration { get; init; } = default!;

    public string GameProviderHangfire { get; init; } = default!;

    public double IntervalMinutes { get; init; } = 3;

    public double IntervalMinutesMax { get; init; } = 60;

    public double IntervalMinutesMinimum { get; init; } = 1;
}
