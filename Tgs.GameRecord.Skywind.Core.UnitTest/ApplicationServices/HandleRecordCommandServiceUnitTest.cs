using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Options;
using NSubstitute;
using Tgs.GameRecord.Handle.Client;
using Tgs.GameRecord.Handle.Common.Protos;
using Tgs.GameRecord.Skywind.Core.ApplicationServices;

namespace Tgs.GameRecord.Skywind.Core.UnitTest.ApplicationServices;

public class HandleRecordCommandServiceUnitTest
{
    [Fact]
    public async void Loss()
    {
        var gameRecordInfo = Options.Create(new GameRecordInfoOptions
        {
            GameProviderCode = "SWTF"
        });
        var fakeClient = Substitute.For<IGameRecordHandleClient>();

        var command = new HandleRecordCommand(
            GameRecord: new GameInterface.SkywindService.Protos.GameRecord
            {
                PlayerCode = "4ugse0rsnufp",
                ExternalRoundId = "rAgbAN57A",
                ExternalGameId = "sw_88sf",
                Finished = true,
                ExternalCreatedOn = DateTimeOffset.Parse("2023-07-06T02:07:59.984+00:00").ToTimestamp(),
                ExternalClosedOn = DateTimeOffset.Parse("2023-07-06T02:08:00.984+00:00").ToTimestamp(),
                Bet = 0.5m,
                ValidBet = 0.5m,
                PlayerWinLoss = -0.5m,
                Turnover = 0.5m,
                Settle = 0,
                AdditionalData = "{\"finished\":true,\"revenue\":0.5,\"currency\":\"CNY\",\"debit\":null,\"credit\":null,\"roundId\":\"rAgbAN57A\",\"brandId\":\"ABwZ8pRb\",\"playerCode\":\"4ugse0rsnufp\",\"gameCode\":\"sw_88sf\",\"device\":\"web\",\"win\":0,\"bet\":0.5,\"balanceBefore\":99.28,\"balanceAfter\":98.78,\"totalEvents\":\"1\",\"firstTs\":\"2023-07-06T02:07:59.984+00:00\",\"ts\":\"2023-07-06T02:07:59.984+00:00\",\"isTest\":false,\"insertedAt\":\"2023-07-06T02:08:00.563+00:00\",\"recoveryType\":null,\"totalJpContribution\":0,\"totalJpWin\":0}"
            });

        var service = new HandleRecordCommandService(
            gameRecordInfo,
            fakeClient);

        await service.ExecuteAsync(command);

        await fakeClient
            .Received(1)
            .MessageAsync(
            gameProviderCode: Arg.Is(gameRecordInfo.Value.GameProviderCode),
            playerCode: Arg.Is(command.GameRecord.PlayerCode),
            wagerState: Arg.Is(WagerStates.Lose),
            wagerType: Arg.Is(WagerTypes.Normal),
            freeGameState: Arg.Is(FreeGameStates.None),
            gambleState: Arg.Is(GambleStates.None),
            cashoutState: Arg.Is(CashoutStates.None),
            resettleState: Arg.Is(ResettleStates.None),
            externalRoundId: Arg.Is(command.GameRecord.ExternalRoundId),
            externalWagerId: Arg.Is(command.GameRecord.ExternalRoundId),
            externalGameId: Arg.Is(command.GameRecord.ExternalGameId),
            externalCreatedOn: Arg.Is(command.GameRecord.ExternalCreatedOn.ToDateTimeOffset()),
            externalClosedOn: Arg.Is(command.GameRecord.ExternalClosedOn.ToDateTimeOffset()),
            externalPayoffOn: Arg.Is(command.GameRecord.ExternalClosedOn.ToDateTimeOffset()),
            externalResettleOn: Arg.Is(DateTimeOffset.UnixEpoch),
            bet: Arg.Is(0.5m),
            validBet: Arg.Is(0.5m),
            settle: Arg.Is(0.0m),
            commission: Arg.Is(0.0m),
            turnover: Arg.Is(0.5m),
            winLoss: Arg.Is(-0.5m),
            additionalData: Arg.Is(command.GameRecord.AdditionalData),
            cancellationToken: Arg.Any<CancellationToken>());
    }

    [Fact]
    public async void Win()
    {
        var gameRecordInfo = Options.Create(new GameRecordInfoOptions
        {
            GameProviderCode = "SWTF"
        });
        var fakeClient = Substitute.For<IGameRecordHandleClient>();

        var command = new HandleRecordCommand(
            GameRecord: new GameInterface.SkywindService.Protos.GameRecord
            {
                PlayerCode = "4ugse0rsnufp",
                ExternalRoundId = "ZQbeQkOON",
                ExternalGameId = "sw_88sf",
                Finished = true,
                ExternalCreatedOn = DateTimeOffset.Parse("2023-07-06T02:08:11.439+00:00").ToTimestamp(),
                ExternalClosedOn = DateTimeOffset.Parse("2023-07-06T02:08:12.528+00:00").ToTimestamp(),
                Bet = 0.5m,
                ValidBet = 0.5m,
                PlayerWinLoss = 1.44m,
                Turnover = 0.5m,
                Settle = 1.94m,
                AdditionalData = "{\"finished\":true,\"revenue\":-1.44,\"currency\":\"CNY\",\"debit\":null,\"credit\":null,\"roundId\":\"ZQbeQkOON\",\"brandId\":\"ABwZ8pRb\",\"playerCode\":\"4ugse0rsnufp\",\"gameCode\":\"sw_88sf\",\"device\":\"web\",\"win\":1.94,\"bet\":0.5,\"balanceBefore\":95.78,\"balanceAfter\":97.22,\"totalEvents\":\"1\",\"firstTs\":\"2023-07-06T02:08:11.439+00:00\",\"ts\":\"2023-07-06T02:08:11.439+00:00\",\"isTest\":false,\"insertedAt\":\"2023-07-06T02:08:12.528+00:00\",\"recoveryType\":null,\"totalJpContribution\":0,\"totalJpWin\":0}"
            });

        var service = new HandleRecordCommandService(
            gameRecordInfo,
            fakeClient);

        await service.ExecuteAsync(command);

        await fakeClient
            .Received(1)
            .MessageAsync(
            gameProviderCode: Arg.Is(gameRecordInfo.Value.GameProviderCode),
            playerCode: Arg.Is(command.GameRecord.PlayerCode),
            wagerState: Arg.Is(WagerStates.Win),
            wagerType: Arg.Is(WagerTypes.Normal),
            freeGameState: Arg.Is(FreeGameStates.None),
            gambleState: Arg.Is(GambleStates.None),
            cashoutState: Arg.Is(CashoutStates.None),
            resettleState: Arg.Is(ResettleStates.None),
            externalRoundId: Arg.Is(command.GameRecord.ExternalRoundId),
            externalWagerId: Arg.Is(command.GameRecord.ExternalRoundId),
            externalGameId: Arg.Is(command.GameRecord.ExternalGameId),
            externalCreatedOn: Arg.Is(command.GameRecord.ExternalCreatedOn.ToDateTimeOffset()),
            externalClosedOn: Arg.Is(command.GameRecord.ExternalClosedOn.ToDateTimeOffset()),
            externalPayoffOn: Arg.Is(command.GameRecord.ExternalClosedOn.ToDateTimeOffset()),
            externalResettleOn: Arg.Is(DateTimeOffset.UnixEpoch),
            bet: Arg.Is(0.5m),
            validBet: Arg.Is(0.5m),
            settle: Arg.Is(1.94m),
            commission: Arg.Is(0.0m),
            turnover: Arg.Is(0.5m),
            winLoss: Arg.Is(1.44m),
            additionalData: Arg.Is(command.GameRecord.AdditionalData),
            cancellationToken: Arg.Any<CancellationToken>());
    }
}
