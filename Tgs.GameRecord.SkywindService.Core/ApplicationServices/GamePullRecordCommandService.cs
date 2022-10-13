using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using Tgs.GameRounds.Common;
using Tgs.GameRounds.Common.MessageModels;
using Tgs.MessageQueue;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.SkywindService.Core.ApplicationServices;

internal class GamePullRecordCommandService : ICommandService<GamePullRecordCommand>
{
    private readonly ILogger<GamePullRecordCommandService> _logger;
    private readonly GameRecordInfoOptions _gameRecordInfo;
    private readonly IMessageSender _messageSender;
    private readonly IUtensil _utensil;
    private readonly string _subject;

    public GamePullRecordCommandService(
        ILogger<GamePullRecordCommandService> logger,
        IOptions<GameRecordInfoOptions> options,
        IMessageSender messageSender,
        IUtensil utensil)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _gameRecordInfo = options.Value;
        _messageSender = messageSender ?? throw new ArgumentNullException(nameof(messageSender));
        _utensil = utensil ?? throw new ArgumentNullException(nameof(utensil));
        _subject = _gameRecordInfo.MongoGlobPattern;
    }

    public async ValueTask ExecuteAsync(GamePullRecordCommand command, CancellationToken cancellationToken = default)
    {
        foreach (var record in command.Records)
        {
            bool success = true;
            var playerModel = await _utensil.GetPlayerAsync(record.PlayerCode).ConfigureAwait(false);
            var gameId = await _utensil.GetGameIdAsync(command.GameProviderId, record.GameCode).ConfigureAwait(false);
            if (playerModel == null || !gameId.HasValue)
            {
                success = false;
            }

            Guid? gameProviderConfigurationId = default;
            if (playerModel != null)
            {
                gameProviderConfigurationId = await _utensil.BrandGetGameProviderConfigurationIdAsync(playerModel.BrandId, command.GameProviderId).ConfigureAwait(false);
                if (!gameProviderConfigurationId.HasValue)
                {
                    success = false;
                }
            }

            var playerWinLoss = record.Win.ToDecimal() - record.Bet.ToDecimal();

            var data = new GameRecordMessage(
                ExternalRoundId: record.RoundId,
                GameProviderId: command.GameProviderId,
                GameProviderCode: command.GameProviderCode,
                GameId: gameId ?? -1,
                ExternalGameId: record.GameCode,
                PlayerCode: (playerModel == null) ? string.Empty : playerModel.PlayerCode,
                CurrencyCode: (playerModel == null) ? string.Empty : playerModel.DefaultWalletCurrencyCode,
                WalletId: (playerModel == null) ? Guid.Empty : playerModel.DefaultWalletId,
                BrandId: (playerModel == null) ? Guid.Empty : playerModel.BrandId,
                GameProviderConfigurationId: gameProviderConfigurationId ?? Guid.Empty,
                Status: GameRoundStatus.Closed,
                ClientStatus: (playerWinLoss > 0) ? GameRoundClientStatus.Win : GameRoundClientStatus.Loss,
                ExternalCreatedOn: DateTimeOffset.Parse(record.FirstTs),
                ExternalClosedOn: DateTimeOffset.Parse(record.Ts),
                BetAmount: record.Bet.ToDecimal().GetValueOrDefault(0),
                ValidBetAmount: record.Bet.ToDecimal().GetValueOrDefault(0),
                SettleAmount: record.Win.ToDecimal().GetValueOrDefault(0),
                Commission: 0,
                Turnover: record.Bet.ToDecimal().GetValueOrDefault(0),
                PlayerWinLoss: playerWinLoss.GetValueOrDefault(0),
                AdditionalData: record.AdditionalData,
                Success: success);

            _logger.LogInformation("LogOn:{logOn}  Subject:{subject} MessageRequest:{request}", DateTime.Now.ToString("o"), _subject, data);

            await _messageSender
                    .PublishAsync(
                        subject: _subject,
                        data: data.ToBson(),
                        cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
        }
    }
}
