using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Logging;
using Tgs.GameInterface.SkywindService.Client;
using Tgs.GameInterface.SkywindService.Protos;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.Skywind.Core.ApplicationServices;

internal class PullRecordBy3rdPartyCommandService : ICommandService<PullRecordBy3rdPartyCommand>
{
    private readonly IGameInterfaceSkywindClient _client;
    private readonly ILogger<PullRecordBy3rdPartyCommandService> _logger;
    private readonly ICommandService<HandleRecordCommand> _service;

    public PullRecordBy3rdPartyCommandService(
        ILogger<PullRecordBy3rdPartyCommandService> logger,
        IGameInterfaceSkywindClient client,
        ICommandService<HandleRecordCommand> service)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _service = service ?? throw new ArgumentNullException(nameof(service));
    }

    public async ValueTask ExecuteAsync(PullRecordBy3rdPartyCommand command, CancellationToken cancellationToken = default)
    {
        var offset = 0;
        var limit = 1000;

        while (true)
        {
            var gameRecordRequest = new GameRecordRequest()
            {
                ApiUrl = command.ApiUrl.ToString(),
                SecretKey = command.SecretKey,
                Username = command.Username,
                Password = command.Password,
                Start = command.StartDate.ToTimestamp(),
                End = command.EndDate.ToTimestamp(),
                Offset = offset,
                Limit = limit
            };
            var gameHistoriesReply = await _client
                .GameRecord3rdPartyAsync(
                    request: gameRecordRequest,
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            _logger.LogInformation("LogOn:{logOn} | Request:{request} | Response:{response}",
                DateTime.UtcNow.ToString("O"),
                command,
                gameHistoriesReply);

            if (gameHistoriesReply == null)
            {
                break;
            }

            var gameRecords = gameHistoriesReply.Items.AsEnumerable() ?? Enumerable.Empty<GameInterface.SkywindService.Protos.GameRecord>();

            if (!gameRecords.Any())
            {
                break;
            }

            offset += limit;

            foreach (var gameRecord in gameRecords)
            {
                try
                {
                    await _service
                        .ExecuteAsync(
                        command: new(gameRecord),
                        cancellationToken: cancellationToken)
                        .ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "LogOn:{logOn} Message: game record exception.({gameRecord})", DateTime.Now.ToString("o"), gameRecord);
                }
            }
        }
    }
}
