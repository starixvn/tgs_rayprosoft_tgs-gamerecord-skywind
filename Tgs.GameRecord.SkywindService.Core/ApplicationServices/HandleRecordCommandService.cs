using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tgs.GameRecord.Handle.Client;
using Tgs.GameRecord.Handle.Common.Protos;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.SkywindService.Core.ApplicationServices;

internal class HandleRecordCommandService : ICommandService<HandleRecordCommand>
{
    private readonly GameRecordInfoOptions _gameRecordInfo;
    private readonly ILogger<HandleRecordCommandService> _logger;
    private readonly IGameRecordHandleClient _workManualClient;

    public HandleRecordCommandService(
        ILogger<HandleRecordCommandService> logger,
        IOptions<GameRecordInfoOptions> options,
        IGameRecordHandleClient workManualClient)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _gameRecordInfo = options.Value;
        _workManualClient = workManualClient ?? throw new ArgumentNullException(nameof(workManualClient));
    }

    public async ValueTask ExecuteAsync(HandleRecordCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var status = GameRecordStatus.Closed;
            var externalCreatedOn = command.GameRecord.ExternalCreatedOn.ToDateTimeOffset();
            var externalClosedOn = command.GameRecord.ExternalClosedOn.ToDateTimeOffset();
            var bet = command.GameRecord.Bet.ToDecimal().GetValueOrDefault(0);
            var validBet = command.GameRecord.Bet.ToDecimal().GetValueOrDefault(0);
            var settle = command.GameRecord.Settle.ToDecimal().GetValueOrDefault(0);
            var commission = 0;
            var turnover = command.GameRecord.Bet.ToDecimal().GetValueOrDefault(0);
            var winLoss = command.GameRecord.PlayerWinLoss.ToDecimal().GetValueOrDefault(0);
            var clientStatus = winLoss <= 0 ? GameRecordClientStatus.Loss : GameRecordClientStatus.Win;

            await _workManualClient
                .MessageAsync(
                gameProviderCode: _gameRecordInfo.GameProviderCode,
                externalRoundId: command.GameRecord.ExternalRoundId,
                externalGameId: command.GameRecord.ExternalGameId,
                playerCode: command.GameRecord.PlayerCode,
                status: status,
                clientStatus: clientStatus,
                externalCreatedOn: externalCreatedOn,
                externalClosedOn: externalClosedOn,
                bet: bet,
                validBet: validBet,
                settle: settle,
                commission: commission,
                turnover: turnover,
                playerWinLoss: winLoss,
                additionalData: command.GameRecord.AdditionalData,
                cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "LogOn:{logOn} Message: An exception occurred in this game record.({gameRecord})", DateTime.Now.ToString("o"), command.GameRecord);
        }
    }
}
