using Microsoft.Extensions.Options;
using Tgs.GameRecord.Handle.Client;
using Tgs.GameRecord.Handle.Common.Protos;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.Skywind.Core.ApplicationServices;

internal class HandleRecordCommandService : ICommandService<HandleRecordCommand>
{
    private readonly IGameRecordHandleClient _client;
    private readonly GameRecordInfoOptions _gameRecordInfo;

    public HandleRecordCommandService(
        IOptions<GameRecordInfoOptions> options,
        IGameRecordHandleClient client)
    {
        _gameRecordInfo = options.Value;
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public ValueTask ExecuteAsync(HandleRecordCommand command, CancellationToken cancellationToken = default)
    {
        var externalCreatedOn = command.GameRecord.ExternalCreatedOn.ToDateTimeOffset();
        var externalClosedOn = command.GameRecord.ExternalClosedOn.ToDateTimeOffset();
        var bet = command.GameRecord.Bet.ToDecimal().GetValueOrDefault(0);
        var validBet = command.GameRecord.Bet.ToDecimal().GetValueOrDefault(0);
        var settle = command.GameRecord.Settle.ToDecimal().GetValueOrDefault(0);
        var commission = 0;
        var turnover = command.GameRecord.Bet.ToDecimal().GetValueOrDefault(0);
        var winLoss = command.GameRecord.PlayerWinLoss.ToDecimal().GetValueOrDefault(0);
        var wagerState = winLoss <= 0 ? WagerStates.Lose : WagerStates.Win;

        return _client
            .MessageAsync(
            gameProviderCode: _gameRecordInfo.GameProviderCode,
            playerCode: command.GameRecord.PlayerCode,
            wagerState: wagerState,
            wagerType: WagerTypes.Normal,
            freeGameState: FreeGameStates.None,
            gambleState: GambleStates.None,
            cashoutState: CashoutStates.None,
            resettleState: ResettleStates.None,
            externalRoundId: command.GameRecord.ExternalRoundId,
            externalWagerId: command.GameRecord.ExternalRoundId,
            externalGameId: command.GameRecord.ExternalGameId,
            externalCreatedOn: externalCreatedOn,
            externalClosedOn: externalClosedOn,
            externalPayoffOn: externalClosedOn,
            externalResettleOn: DateTimeOffset.UnixEpoch,
            bet: bet,
            validBet: validBet,
            settle: settle,
            commission: commission,
            turnover: turnover,
            winLoss: winLoss,
            additionalData: command.GameRecord.AdditionalData,
            cancellationToken: cancellationToken);
    }
}
