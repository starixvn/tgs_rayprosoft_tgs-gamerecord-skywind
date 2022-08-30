namespace Tgs.GameRecord.SkywindService;

public record ScheduleMessageModel(
    Uri GameApiUrl,
    string SecretKey,
    string Username,
    string Password);
