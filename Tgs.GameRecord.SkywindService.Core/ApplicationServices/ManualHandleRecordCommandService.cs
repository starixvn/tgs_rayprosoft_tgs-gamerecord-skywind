using System.Text.Json;
using Microsoft.Extensions.Logging;
using Tgs.GameRounds.Work.Manual.Client;
using Tgs.GameRounds.Work.Manual.Common.Protos;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.SkywindService.Core.ApplicationServices;

internal class ManualHandleRecordCommandService : ICommandService<ManualHandleRecordCommand>
{
    private readonly ILogger<ManualHandleRecordCommandService> _logger;
    private readonly IUtensil _utensil;
    private readonly IGameRoundsWorkManualClient _workManualClient;

    public ManualHandleRecordCommandService(
        ILogger<ManualHandleRecordCommandService> logger,
        IUtensil utensil,
        IGameRoundsWorkManualClient workManualClient)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _utensil = utensil ?? throw new ArgumentNullException(nameof(utensil));
        _workManualClient = workManualClient ?? throw new ArgumentNullException(nameof(workManualClient));
    }

    public async ValueTask ExecuteAsync(ManualHandleRecordCommand command, CancellationToken cancellationToken = default)
    {
        var gameProviderId = await _utensil
            .GetGameProviderIdAsync(
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        var gameProviderCode = _utensil
            .GetGameProviderCode();
        var record = command.GameRecord;
        try
        {
            bool success = true;
            var playerModel = await _utensil
                .GetPlayerAsync(
                    playerCode: record.PlayerCode,
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);
            var gameId = await _utensil
                .GetGameIdAsync(
                    gameProviderId: gameProviderId,
                    gameExternalId: record.ExternalGameId,
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);
            if (playerModel == null || !gameId.HasValue)
            {
                success = false;
            }

            Guid? gameProviderConfigurationId = default;
            if (playerModel != null)
            {
                gameProviderConfigurationId = await _utensil
                    .GetBrandGameProviderConfigurationIdAsync(
                        brandId: playerModel.BrandId,
                        gameProviderId: gameProviderId,
                        cancellationToken: cancellationToken)
                    .ConfigureAwait(false);

                if (!gameProviderConfigurationId.HasValue)
                {
                    success = false;
                }
            }

            if (success)
            {
                var status = RoundStatus.Closed;
                var externalCreatedOn = record.ExternalCreatedOn.ToDateTimeOffset();
                var externalClosedOn = record.ExternalClosedOn.ToDateTimeOffset();
                var bet = record.Bet.ToDecimal().GetValueOrDefault(0);
                var validBet = record.Bet.ToDecimal().GetValueOrDefault(0);
                var settle = record.Settle.ToDecimal().GetValueOrDefault(0);
                var commission = 0;
                var turnover = record.Bet.ToDecimal().GetValueOrDefault(0);
                var winLoss = record.PlayerWinLoss.ToDecimal().GetValueOrDefault(0);
                var clientStatus = winLoss <= 0 ? RoundClientStatus.Loss : RoundClientStatus.Win;

                await _workManualClient
                    .RoundBidding(
                        gameProviderId: gameProviderId,
                        gameProviderCode: gameProviderCode,
                        externalRoundId: record.ExternalRoundId,
                        gameId: gameId!.Value,
                        externalGameId: record.ExternalGameId,
                        playerCode: playerModel!.PlayerCode,
                        walletId: playerModel!.DefaultWalletId,
                        currencyCode: playerModel!.DefaultWalletCurrencyCode,
                        brandId: playerModel!.BrandId,
                        gameProviderConfigurationId: gameProviderConfigurationId!.Value,
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
                        additionalData: record.AdditionalData,
                        cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
            }
            else
            {
                await _workManualClient
                    .RoundMissing(
                        gameProviderId: gameProviderId,
                        gameProviderCode: gameProviderCode,
                        externalRoundId: record.ExternalRoundId,
                        gameId: gameId ?? -1,
                        externalGameId: record.ExternalGameId,
                        playerCode: playerModel?.PlayerCode ?? string.Empty,
                        data: JsonSerializer.Serialize(record),
                        cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "遊戲紀錄處理發生例外.({record})", record);
        }
    }
}
