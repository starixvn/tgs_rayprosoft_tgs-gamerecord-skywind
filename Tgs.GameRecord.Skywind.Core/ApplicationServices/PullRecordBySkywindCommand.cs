namespace Tgs.GameRecord.Skywind.Core.ApplicationServices;

public record PullRecordBySkywindCommand(
    Uri ApiUrl,
    string SecretKey,
    string Username,
    string Password,
    DateTimeOffset StartTime,
    DateTimeOffset EndTime);
