namespace Tgs.GameRecord.Skywind.Core.ApplicationServices;

public record PullRecordCommand(
	Uri ApiUrl,
	string SecretKey,
	string Username,
	string Password,
	DateTimeOffset StartTime,
	DateTimeOffset EndTime);
