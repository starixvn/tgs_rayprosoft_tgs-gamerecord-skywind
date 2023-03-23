namespace Tgs.GameRecord.SkywindService.Messages;

public record ScheduleMessageModel(
    Uri GameApiUrl,
    string SecretKey,
    string Username,
    string Password);
