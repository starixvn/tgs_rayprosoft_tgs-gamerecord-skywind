using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tgs.GameInterface.SkywindService.Client;
using Tgs.GameInterface.SkywindService.Protos;
using Tgs.GameRecord.SkywindService.Core.ApplicationServices;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.SkywindService.Core.WorkerServices;

internal class SkywindSettleWorkerService : ICommandService<SkywindSettleWorkerCommand>
{
    private readonly IGameInterfaceSkywindClient _client;
    private readonly GameRecordInfoOptions _gameRecordInfo;
    private readonly ILogger<SkywindSettleWorkerService> _logger;
    private readonly ICommandService<GamePullRecordCommand> _service;
    private readonly IUtensil _utensil;

    public SkywindSettleWorkerService(
        IOptions<GameRecordInfoOptions> options,
        IGameInterfaceSkywindClient client,
        ILogger<SkywindSettleWorkerService> logger,
        IUtensil utensil,
        ICommandService<GamePullRecordCommand> service)
    {
        _gameRecordInfo = options.Value;
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _utensil = utensil ?? throw new ArgumentNullException(nameof(utensil));
        _service = service ?? throw new ArgumentNullException(nameof(service));
    }

    public async ValueTask ExecuteAsync(SkywindSettleWorkerCommand command, CancellationToken cancellationToken = default)
    {
        var gameProviderId = await _utensil.GetGameProviderIdAsync(_gameRecordInfo.GameProviderCode).ConfigureAwait(false);
        var offset = 0;
        var limit = 1000;

        while (true)
        {
            var gameRecordRequest = new GameRecordRequest()
            {
                ApiUrl = command.GameApiUrl.ToString(),
                SecretKey = command.SecretKey,
                Username = command.Username,
                Password = command.Password,
                Start = command.StartDate.ToTimestamp(),
                End = command.EndDate.ToTimestamp(),
                Offset = offset,
                Limit = limit
            };
            var gameHistoriesReply = await _client.GameRecordBySettleAsync(
                request: gameRecordRequest,
               cancellationToken: cancellationToken).ConfigureAwait(false);

            _logger.LogInformation("LogOn:{logOn} | Request:{request} | Response:{response}",
                DateTime.UtcNow.ToString("O"),
                command,
                gameHistoriesReply);

            if (gameHistoriesReply == null)
            {
                break;
            }

            var records = gameHistoriesReply.Items.AsEnumerable() ?? Enumerable.Empty<GameInterface.SkywindService.Protos.GameRecord>();

            if (!records.Any())
            {
                break;
            }

            offset += limit;

            await _service.ExecuteAsync(new GamePullRecordCommand(
                GameProviderId: gameProviderId,
                GameProviderCode: _gameRecordInfo.GameProviderCode,
                Records: records), cancellationToken: cancellationToken).ConfigureAwait(false);
        }
    }
}
