using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.Skywind.Core.ApplicationServices;

internal class PullRecordCommandSerivce(
	ILogger<PullRecordCommandSerivce> logger,
	ICommandService<PullRecordBySkywindCommand> pullRecordBySkywindCommandService,
	ICommandService<PullRecordBy3rdPartyCommand> pullRecordBy3rdPartyService) : ICommandService<PullRecordCommand>
{
	public async ValueTask ExecuteAsync(PullRecordCommand command, CancellationToken cancellationToken = default)
	{
		try
		{
			var pullRecordBySkywindCommand = new PullRecordBySkywindCommand(
				ApiUrl: command.ApiUrl,
				SecretKey: command.SecretKey,
				Username: command.Username,
				Password: command.Password,
				StartTime: command.StartTime,
				EndTime: command.EndTime);

			logger.LogInformation("LogOn:{logOn} execute the command:{command}", DateTime.Now.ToString("o"), command);

			await pullRecordBySkywindCommandService.ExecuteAsync(
				command: pullRecordBySkywindCommand,
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "LogOn:{logOn}  An exception occurred in the process of game account pulling.", DateTime.Now.ToString("o"));
		}

		try
		{
			var pullRecordBy3RdPartyCommand = new PullRecordBy3rdPartyCommand(
				ApiUrl: command.ApiUrl,
				SecretKey: command.SecretKey,
				Username: command.Username,
				Password: command.Password,
				StartTime: command.StartTime,
				EndTime: command.EndTime);

			logger.LogInformation("LogOn:{logOn} execute the command:{command}", DateTime.Now.ToString("o"), command);

			await pullRecordBy3rdPartyService.ExecuteAsync(
				command: pullRecordBy3RdPartyCommand,
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "LogOn:{logOn}  An exception occurred in the 3rdParty game account opening process.", DateTime.Now.ToString("o"));
		}
	}
}
