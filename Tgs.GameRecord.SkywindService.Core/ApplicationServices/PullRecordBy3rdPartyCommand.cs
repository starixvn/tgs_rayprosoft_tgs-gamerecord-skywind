namespace Tgs.GameRecord.SkywindService.Core.ApplicationServices;

public record PullRecordBy3rdPartyCommand(
    Uri ApiUrl,
    string SecretKey,
    string Username,
    string Password,
    DateTimeOffset StartDate,
    DateTimeOffset EndDate);
