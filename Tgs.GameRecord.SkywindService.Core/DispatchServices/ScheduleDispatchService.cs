using Microsoft.Extensions.Options;
using Tgs.GameRecord.SkywindService.Core.WorkerServices;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.SkywindService.Core.DispatchServices;

internal class ScheduleDispatchService : ICommandService<ScheduleDispatchCommand>
{
    private readonly ICommandService<SkywindSettleWorkerCommand> _commandService;
    private readonly GameRecordInfoOptions _gameRecordInfoOptions;
    private readonly double _intervalMinute;
    private readonly ICommandService<Skywind3rdPartyWorkerCommand> _partycommandService;
    private readonly IScheduleDispatch _scheduleDispatch;

    public ScheduleDispatchService(
        IOptions<GameRecordInfoOptions> options,
        ICommandService<SkywindSettleWorkerCommand> commandService,
        ICommandService<Skywind3rdPartyWorkerCommand> partycommandService,
        IScheduleDispatch scheduleDispatch
        )
    {
        _gameRecordInfoOptions = options.Value;
        _commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));
        _partycommandService = partycommandService ?? throw new ArgumentNullException(nameof(partycommandService));
        _scheduleDispatch = scheduleDispatch ?? throw new ArgumentNullException(nameof(scheduleDispatch));
        _intervalMinute = (_gameRecordInfoOptions.IntervalMinutes < _gameRecordInfoOptions.IntervalMinutesMinimum)
                        ? _gameRecordInfoOptions.IntervalMinutesMinimum
                        : _gameRecordInfoOptions.IntervalMinutes;
    }

    public async ValueTask ExecuteAsync(ScheduleDispatchCommand command, CancellationToken cancellationToken = default)
    {
        var key = $"gp:gamerecord:{_gameRecordInfoOptions.GameProviderCode}:{command.Username}".ToLower();
        if (!await _scheduleDispatch.ExistAsync(key: key).ConfigureAwait(false))
        {
            if (await _scheduleDispatch.LockingAsync(key: key, command: command).ConfigureAwait(false))
            {
                try
                {
                    var endDate = DateTimeOffset.ParseExact(DateTimeOffset.UtcNow.ToString(@"yyyy-MM-dd\THH:mm:00K"), @"yyyy-MM-dd\THH:mm:ssK", null, styles: System.Globalization.DateTimeStyles.RoundtripKind);

                    var startDate = endDate.AddMinutes(_intervalMinute * -1);

                    await _commandService.ExecuteAsync(new SkywindSettleWorkerCommand(
                         GameApiUrl: command.GameApiUrl,
                         SecretKey: command.SecretKey,
                         Username: command.Username,
                         Password: command.Password,
                         StartDate: startDate,
                         EndDate: endDate), cancellationToken: cancellationToken).ConfigureAwait(false);

                    await _partycommandService.ExecuteAsync(new Skywind3rdPartyWorkerCommand(
                         GameApiUrl: command.GameApiUrl,
                         SecretKey: command.SecretKey,
                         Username: command.Username,
                         Password: command.Password,
                         StartDate: startDate,
                         EndDate: endDate), cancellationToken: cancellationToken).ConfigureAwait(false);
                }
                finally
                {
                    await _scheduleDispatch.UnlockAsync(key: key).ConfigureAwait(false);
                }
            }
        }
    }
}
