using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Logging;
using Tgs.GameInterface.SkywindService.Client;
using Tgs.GameInterface.SkywindService.Protos;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.Skywind.Core.ApplicationServices;

internal class PullRecordBy3rdPartyCommandService(
	ILogger<PullRecordBy3rdPartyCommandService> logger,
	IGameInterfaceSkywindClient client,
	ICommandService<HandleRecordCommand> service) : ICommandService<PullRecordBy3rdPartyCommand>
{
	public async ValueTask ExecuteAsync(PullRecordBy3rdPartyCommand command, CancellationToken cancellationToken = default)
	{
		var offset = 0;
		var limit = 1000;
		IEnumerable<GameInterface.SkywindService.Protos.GameRecord> gameRecords;

		do
		{
			var gameRecordRequest = new GameRecordRequest
			{
				ApiUrl = command.ApiUrl.ToString(),
				SecretKey = command.SecretKey,
				Username = command.Username,
				Password = command.Password,
				Start = command.StartTime.ToTimestamp(),
				End = command.EndTime.ToTimestamp(),
				Offset = offset,
				Limit = limit
			};

			var gameHistoriesReply = await client.GameRecord3rdPartyAsync(
				request: gameRecordRequest,
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			gameRecords = gameHistoriesReply.Items.AsEnumerable() ?? Enumerable.Empty<GameInterface.SkywindService.Protos.GameRecord>();

			foreach (var gameRecord in gameRecords)
			{
				try
				{
					await service
						.ExecuteAsync(
						command: new(gameRecord),
						cancellationToken: cancellationToken)
						.ConfigureAwait(false);
				}
				catch (Exception ex)
				{
					logger.LogError(ex, "LogOn:{logOn} Message: game record exception.({gameRecord})", DateTime.Now.ToString("o"), gameRecord);
				}
			}

			offset += limit;
		} while (gameRecords.Any());
	}
}
