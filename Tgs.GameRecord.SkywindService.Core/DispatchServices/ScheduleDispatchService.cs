using Microsoft.Extensions.Logging;
using Tgs.GameRecord.SkywindService.Core.ApplicationServices;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.SkywindService.Core.DispatchServices;

internal class ScheduleDispatchService : ICommandService<ScheduleDispatchCommand>
{
    private readonly ILogger<ScheduleDispatchService> _logger;
    private readonly ICommandService<Schedule3rdPartyPullRecordCommand> _schedule3RdPartyService;
    private readonly ICommandService<SchedulePullRecordCommand> _scheduleService;

    public ScheduleDispatchService(
        ILogger<ScheduleDispatchService> logger,
        ICommandService<SchedulePullRecordCommand> scheduleService,
        ICommandService<Schedule3rdPartyPullRecordCommand> schedule3rdPartyService
        )
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _scheduleService = scheduleService ?? throw new ArgumentNullException(nameof(scheduleService));
        _schedule3RdPartyService = schedule3rdPartyService ?? throw new ArgumentNullException(nameof(schedule3rdPartyService));
    }

    public async ValueTask ExecuteAsync(ScheduleDispatchCommand command, CancellationToken cancellationToken = default)
    {
        var endDate = DateTimeOffset.ParseExact(DateTimeOffset.UtcNow.ToString(@"yyyy-MM-dd\THH:mm:00K"), @"yyyy-MM-dd\THH:mm:ssK", null, styles: System.Globalization.DateTimeStyles.RoundtripKind);

        var startDate = endDate.AddMinutes(3 * -1);

        var schedulePullRecordCommand = new SchedulePullRecordCommand(
            ApiUrl: command.GameApiUrl,
            SecretKey: command.SecretKey,
            Username: command.Username,
            Password: command.Password,
            StartDate: startDate,
            EndDate: endDate);

        _logger.LogInformation("LogOn:{logOn} execute the command:{command}", DateTime.Now.ToString("o"), command);

        await _scheduleService
            .ExecuteAsync(
                command: schedulePullRecordCommand,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var schedule3RdPartyPullRecordCommand = new Schedule3rdPartyPullRecordCommand(
            ApiUrl: command.GameApiUrl,
            SecretKey: command.SecretKey,
            Username: command.Username,
            Password: command.Password,
            StartDate: startDate,
            EndDate: endDate);

        _logger.LogInformation("LogOn:{logOn} execute the command:{command}", DateTime.Now.ToString("o"), command);

        await _schedule3RdPartyService
            .ExecuteAsync(
                command: schedule3RdPartyPullRecordCommand,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }
}
