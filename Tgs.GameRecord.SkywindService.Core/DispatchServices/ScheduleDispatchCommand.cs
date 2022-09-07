namespace Tgs.GameRecord.SkywindService.Core.DispatchServices;

public record ScheduleDispatchCommand(
    Uri GameApiUrl,
    string SecretKey,
    string Username,
    string Password);
