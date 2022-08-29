namespace Tgs.GameRecord.SkywindService.Core;

public record GameRecordManualCommand(
    Uri GameRecordApiUrl,
    string GameProviderAgentId,
    string GameProviderCert,
    DateTimeOffset StartTime,
    DateTimeOffset EndTime);
