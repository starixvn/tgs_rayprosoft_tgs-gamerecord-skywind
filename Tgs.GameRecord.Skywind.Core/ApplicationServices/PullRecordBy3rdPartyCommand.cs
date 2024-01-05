namespace Tgs.GameRecord.Skywind.Core.ApplicationServices;

public record PullRecordBy3rdPartyCommand(
    Uri ApiUrl,
    string SecretKey,
    string Username,
    string Password,
    DateTimeOffset StartTime,
    DateTimeOffset EndTime);
