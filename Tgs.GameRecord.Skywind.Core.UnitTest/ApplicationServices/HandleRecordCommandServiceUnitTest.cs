using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using Tgs.GameRecord.Handle.Client;
using Tgs.GameRecord.Skywind.Core.ApplicationServices;
using Valhalla.MessageQueue;

namespace Tgs.GameRecord.Skywind.Core.UnitTest.ApplicationServices;

public class HandleRecordCommandServiceUnitTest
{
	[Fact]
	public async Task Loss()
	{
		var faceLogger = NullLogger<HandleRecordCommandService>.Instance;
		var fakeGameRecordInfoOptions = Options.Create(new GameRecordInfoOptions
		{
			GameProviderCode = "SWTF",
			GameProviderConfiguration = "SWTFConfiguration",
			HangfirePrefix = "gamerecord-sw-hangfire:",
			HangfireLocking = "gamerecord-sw-locking:",
			IntervalMinutes = 3,
			IntervalMinutesMax = 15,
			IntervalMinutesMinimum = 1
		});
		var fakePolicySettingsOptions = Options.Create(new PolicySettingsOptions
		{
			RetryCount = 1,
			RetrySeconds = 1,
		});
		var fakeGameRecordHandleClient = Substitute.For<IGameRecordHandleClient>();

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

		var sut = new HandleRecordCommandService(
			faceLogger,
			fakeGameRecordInfoOptions,
			fakePolicySettingsOptions,
			fakeGameRecordHandleClient);

		await sut.ExecuteAsync(command);

		_ = fakeGameRecordHandleClient
			.Received(1)
			.HandleAsync(
			source: Arg.Is(new Handle.Common.ViewModels.GameRecordHandleViewModel
			{
				GameProviderCode = fakeGameRecordInfoOptions.Value.GameProviderCode,
				PlayerCode = command.GameRecord.PlayerCode,
				ExternalRoundId = command.GameRecord.ExternalRoundId,
				ExternalBetId = command.GameRecord.ExternalRoundId,
				ExternalGameId = command.GameRecord.ExternalGameId,
				ExternalCreatedOn = command.GameRecord.ExternalCreatedOn.ToDateTimeOffset(),
				ExternalClosedOn = command.GameRecord.ExternalClosedOn.ToDateTimeOffset(),
				ExternalPayoffOn = command.GameRecord.ExternalClosedOn.ToDateTimeOffset(),
				ExternalResettleOn = DateTimeOffset.UnixEpoch,
				BetState = Handle.Common.Protos.BetState.Loss,
				BetType = Handle.Common.Protos.BetType.Normal,
				FreeGameState = Handle.Common.Protos.FreeGameState.None,
				GambleState = Handle.Common.Protos.GambleState.None,
				CashoutState = Handle.Common.Protos.CashoutState.None,
				ResettleState = Handle.Common.Protos.ResettleState.None,
				JackpotState = Handle.Common.Protos.JackpotState.None,
				Bet = 0.5m,
				ValidBet = 0.5m,
				Settle = 0.0M,
				Commission = 0.0M,
				Turnover = 0.5m,
				WinLoss = -0.5m,
				Jackpot = 0.0M,
				AdditionalData = command.GameRecord.AdditionalData
			}),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task Win()
	{
		var faceLogger = NullLogger<HandleRecordCommandService>.Instance;
		var fakeGameRecordInfoOptions = Options.Create(new GameRecordInfoOptions
		{
			GameProviderCode = "SWTF",
			GameProviderConfiguration = "SWTFConfiguration",
			HangfirePrefix = "gamerecord-sw-hangfire:",
			HangfireLocking = "gamerecord-sw-locking:",
			IntervalMinutes = 3,
			IntervalMinutesMax = 15,
			IntervalMinutesMinimum = 1
		});
		var fakePolicySettingsOptions = Options.Create(new PolicySettingsOptions
		{
			RetryCount = 1,
			RetrySeconds = 1,
		});
		var fakeGameRecordHandleClient = Substitute.For<IGameRecordHandleClient>();

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

		var sut = new HandleRecordCommandService(
			faceLogger,
			fakeGameRecordInfoOptions,
			fakePolicySettingsOptions,
			fakeGameRecordHandleClient);

		await sut.ExecuteAsync(command);

		_ = fakeGameRecordHandleClient
			.Received(1)
			.HandleAsync(
			source: Arg.Is(new Handle.Common.ViewModels.GameRecordHandleViewModel
			{
				GameProviderCode = fakeGameRecordInfoOptions.Value.GameProviderCode,
				PlayerCode = command.GameRecord.PlayerCode,
				ExternalRoundId = command.GameRecord.ExternalRoundId,
				ExternalBetId = command.GameRecord.ExternalRoundId,
				ExternalGameId = command.GameRecord.ExternalGameId,
				ExternalCreatedOn = command.GameRecord.ExternalCreatedOn.ToDateTimeOffset(),
				ExternalClosedOn = command.GameRecord.ExternalClosedOn.ToDateTimeOffset(),
				ExternalPayoffOn = command.GameRecord.ExternalClosedOn.ToDateTimeOffset(),
				ExternalResettleOn = DateTimeOffset.UnixEpoch,
				BetState = Handle.Common.Protos.BetState.Win,
				BetType = Handle.Common.Protos.BetType.Normal,
				FreeGameState = Handle.Common.Protos.FreeGameState.None,
				GambleState = Handle.Common.Protos.GambleState.None,
				CashoutState = Handle.Common.Protos.CashoutState.None,
				ResettleState = Handle.Common.Protos.ResettleState.None,
				JackpotState = Handle.Common.Protos.JackpotState.None,
				Bet = 0.5m,
				ValidBet = 0.5m,
				Settle = 1.94m,
				Commission = 0.0M,
				Turnover = 0.5m,
				WinLoss = 1.44m,
				Jackpot = 0.0M,
				AdditionalData = command.GameRecord.AdditionalData
			}),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}
