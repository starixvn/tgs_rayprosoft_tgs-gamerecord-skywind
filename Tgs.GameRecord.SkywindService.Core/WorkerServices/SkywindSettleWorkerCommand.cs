namespace Tgs.GameRecord.SkywindService.Core.WorkerServices;

public record SkywindSettleWorkerCommand(
    Uri GameApiUrl,
    string SecretKey,
    string Username,
    string Password,
    DateTimeOffset StartDate,
    DateTimeOffset EndDate);
