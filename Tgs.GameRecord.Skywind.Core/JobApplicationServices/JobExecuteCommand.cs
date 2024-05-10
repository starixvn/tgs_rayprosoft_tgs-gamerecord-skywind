namespace Tgs.GameRecord.Skywind.Core.JobApplicationServices;

public record JobExecuteCommand(
	string GameProviderCode,
	string Name,
	Uri ApiUrl,
	string SecretKey,
	string Username,
	string Password);
