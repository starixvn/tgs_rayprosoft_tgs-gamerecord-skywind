namespace Tgs.GameRecord.SkywindService.Core.ApplicationServices;

public record ManualPullRecordCommand(
    Uri ApiUrl,
    string SecretKey,
    string Username,
    string Password,
    DateTimeOffset StartDate,
    DateTimeOffset EndDate);
