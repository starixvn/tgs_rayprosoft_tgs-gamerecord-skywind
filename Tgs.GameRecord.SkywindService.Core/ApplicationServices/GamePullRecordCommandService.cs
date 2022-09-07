using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using Tgs.MessageQueue;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.SkywindService.Core.ApplicationServices;

internal class GamePullRecordCommandService : ICommandService<GamePullRecordCommand>
{
    private readonly ILogger<GamePullRecordCommandService> _logger;
    private readonly GameRecordInfoOptions _gameRecordInfoOptions;
    private readonly IMessageSender _messageSender;
    private readonly IUtensil _utensil;

    public GamePullRecordCommandService(
        ILogger<GamePullRecordCommandService> logger,
        IOptions<GameRecordInfoOptions> options,
        IMessageSender messageSender,
        IUtensil utensil)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _gameRecordInfoOptions = options.Value;
        _messageSender = messageSender ?? throw new ArgumentNullException(nameof(messageSender));
        _utensil = utensil ?? throw new ArgumentNullException(nameof(utensil));
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

            var data = new GameRoundData(
                ExternalRoundId: record.RoundId,
                GameProviderId: command.GameProviderId,
                GameProviderCode: command.GameProviderCode,
                PlayerCode: (playerModel == null) ? string.Empty : playerModel.PlayerCode,
                GameId: gameId ?? -1,
                ExternalGameId: record.GameCode,
                CurrencyCode: (playerModel == null) ? string.Empty : playerModel.DefaultWalletCurrencyCode,
                Status: GameRoundStatus.Closed,
                GameProviderConfigurationId: gameProviderConfigurationId ?? Guid.Empty,
                WalletId: (playerModel == null) ? Guid.Empty : playerModel.DefaultWalletId,
                ExternalCreatedOn: DateTimeOffset.Parse(record.FirstTs),
                ExternalClosedOn: DateTimeOffset.Parse(record.Ts),
                BrandId: (playerModel == null) ? Guid.Empty : playerModel.BrandId,
                BetAmount: record.Bet.ToDecimal(),
                ValidBetAmount: record.Bet.ToDecimal(),
                SettleAmount: record.Win.ToDecimal(),
                Commission: 0,
                Turnover: record.Bet.ToDecimal(),
                PlayerWinLoss: playerWinLoss,
                AdditionalData: record.AdditionalData,
                ClientStatus: (playerWinLoss > 0) ? GameRoundClientStatus.Win : GameRoundClientStatus.Loss,
                Success: success);

            var subject = $"tgs.gp.gamerecord.database.round.{_gameRecordInfoOptions.GameProviderCode}".ToLower();

            _logger.LogInformation("LogOn:{logOn}  Subject:{subject} MessageRequest:{request}", DateTime.Now.ToString("o"), subject, data);

            await _messageSender
                    .PublishAsync(
                        subject: $"tgs.gp.gamerecord.database.round.{_gameRecordInfoOptions.GameProviderCode}".ToLower(),
                        data: data.ToBson(),
                        cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
        }
    }
}
