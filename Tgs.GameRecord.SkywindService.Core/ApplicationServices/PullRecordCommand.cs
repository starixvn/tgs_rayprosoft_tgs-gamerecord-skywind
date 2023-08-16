namespace Tgs.GameRecord.SkywindService.Core.ApplicationServices;

public record PullRecordCommand(
    Uri ApiUrl,
    string SecretKey,
    string Username,
    string Password,
    DateTimeOffset StartDate,
    DateTimeOffset EndDate);
