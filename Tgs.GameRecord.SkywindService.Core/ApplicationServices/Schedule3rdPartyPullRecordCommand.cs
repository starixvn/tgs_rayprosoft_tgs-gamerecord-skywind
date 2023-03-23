namespace Tgs.GameRecord.SkywindService.Core.ApplicationServices;

public record Schedule3rdPartyPullRecordCommand(
    Uri ApiUrl,
    string SecretKey,
    string Username,
    string Password,
    DateTimeOffset StartDate,
    DateTimeOffset EndDate);
