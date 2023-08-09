using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Logging;
using Tgs.GameInterface.SkywindService.Client;
using Tgs.GameInterface.SkywindService.Protos;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.SkywindService.Core.ApplicationServices;

internal class Schedule3rdPartyPullRecordCommandService : ICommandService<Schedule3rdPartyPullRecordCommand>
{
    private readonly IGameInterfaceSkywindClient _client;
    private readonly ILogger<Schedule3rdPartyPullRecordCommandService> _logger;
    private readonly ICommandService<HandleRecordCommand> _service;

    public Schedule3rdPartyPullRecordCommandService(
        ILogger<Schedule3rdPartyPullRecordCommandService> logger,
        IGameInterfaceSkywindClient client,
        ICommandService<HandleRecordCommand> service)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _service = service ?? throw new ArgumentNullException(nameof(service));
    }

    public async ValueTask ExecuteAsync(Schedule3rdPartyPullRecordCommand command, CancellationToken cancellationToken = default)
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
                await _service
                    .ExecuteAsync(
                        command: new(gameRecord),
                        cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
            }
        }
    }
}
