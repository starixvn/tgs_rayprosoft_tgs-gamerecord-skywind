namespace Tgs.GameRecord.SkywindService;

public record ManualMessageRequest(
    Uri GameApiUrl,
    string SecretKey,
    string Username,
    string Password,
    DateTimeOffset StartDate,
    DateTimeOffset EndDate);
