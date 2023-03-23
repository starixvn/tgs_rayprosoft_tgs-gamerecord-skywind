using System.Text.Json;
using Microsoft.Extensions.Logging;
using Tgs.GameRecord.SkywindService.Core.ApplicationServices;
using Tgs.GameRounds.Schedule.Client;
using Tgs.GameRounds.Schedule.Common.Protos;
using Tgs.GameRounds.Schedule.Protos;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.SkywindService.Core;

internal class ScheduleFormDispatcher : IScheduleFormDispatcher
{
    private readonly IGameRoundsScheduleClient _clinet;
    private readonly ILogger<ScheduleFormDispatcher> _logger;
    private readonly ICommandService<Manual3rdPartyPullRecordCommand> _manual3RdPartyService;
    private readonly ICommandService<ManualPullRecordCommand> _manualService;
    private readonly ICommandService<Schedule3rdPartyPullRecordCommand> _schedule3RdPartyService;
    private readonly IScheduleFormLock _scheduleFormLock;
    private readonly ICommandService<SchedulePullRecordCommand> _scheduleService;

    public ScheduleFormDispatcher(
        ILogger<ScheduleFormDispatcher> logger,
        IGameRoundsScheduleClient clinet,
        IScheduleFormLock scheduleFormLock,
        ICommandService<SchedulePullRecordCommand> scheduleService,
        ICommandService<Schedule3rdPartyPullRecordCommand> schedule3rdPartyService,
        ICommandService<ManualPullRecordCommand> manualService,
        ICommandService<Manual3rdPartyPullRecordCommand> manual3rdPartyService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _clinet = clinet ?? throw new ArgumentNullException(nameof(clinet));
        _scheduleFormLock = scheduleFormLock ?? throw new ArgumentNullException(nameof(scheduleFormLock));
        _scheduleService = scheduleService ?? throw new ArgumentNullException(nameof(scheduleService));
        _schedule3RdPartyService = schedule3rdPartyService ?? throw new ArgumentNullException(nameof(schedule3rdPartyService));
        _manualService = manualService ?? throw new ArgumentNullException(nameof(manualService));
        _manual3RdPartyService = manual3rdPartyService ?? throw new ArgumentNullException(nameof(manual3rdPartyService));
    }

    public async ValueTask ExecuteAsync(
        string gameProviderCode,
        long id,
        CancellationToken cancellationToken = default)
    {
        var scheduleForm = await _clinet
            .ScheduleFormGetAsync(
                gameProviderCode: gameProviderCode,
                id: id,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false)
            ?? throw new Exception($"The game provider ({gameProviderCode}) cannot find the schedule form. ({id})");

        if (scheduleForm.State != ScheduleFormState.Queued)
        {
            _logger.LogWarning("This game provider ({gameProviderCode}) schedule form status is not queued.({id})", gameProviderCode, id);
            return;
        }

        if (!await _scheduleFormLock
            .LockingAsync(
                id: id)
            .ConfigureAwait(false))
        {
            _logger.LogWarning("LogOn:{logOn} Message:This game provider ({gameProviderCode}) schedule form has been locked. ({id})",
                DateTime.Now.ToString("o"),
                gameProviderCode,
                id);
            return;
        }

        try
        {
            await _clinet
                .ScheduleFormProcessingAsync(
                    gameProviderCode: gameProviderCode,
                    id: id,
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            var parameter = JsonSerializer.Deserialize<SkywindParameter>(scheduleForm.Parameters)
                ?? throw new Exception($"This game provider ({gameProviderCode}) scheduling form parameter is null. ({id})");

            if (scheduleForm.Source == ScheduleFormSource.Schedule)
            {
                var command = new SchedulePullRecordCommand(
                    ApiUrl: new Uri(parameter.ApiUrl),
                    SecretKey: parameter.SecretKey,
                    Username: parameter.Username,
                    Password: parameter.Password,
                    StartDate: parameter.StartTime.ToDateTimeOffset(),
                    EndDate: parameter.EndTime.ToDateTimeOffset());

                _logger.LogInformation("LogOn:{logOn} execute the command:{command}", DateTime.Now.ToString("o"), command);

                await _scheduleService
                    .ExecuteAsync(
                        command: command,
                        cancellationToken: cancellationToken)
                    .ConfigureAwait(false);

                var schedule3RdPartyPullRecordCommand = new Schedule3rdPartyPullRecordCommand(
                    ApiUrl: new Uri(parameter.ApiUrl),
                    SecretKey: parameter.SecretKey,
                    Username: parameter.Username,
                    Password: parameter.Password,
                    StartDate: parameter.StartTime.ToDateTimeOffset(),
                    EndDate: parameter.EndTime.ToDateTimeOffset());

                _logger.LogInformation("LogOn:{logOn} execute the command:{command}", DateTime.Now.ToString("o"), command);

                await _schedule3RdPartyService
                    .ExecuteAsync(
                        command: schedule3RdPartyPullRecordCommand,
                        cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
            }
            else
            {
                var command = new ManualPullRecordCommand(
                    ApiUrl: new Uri(parameter.ApiUrl),
                    SecretKey: parameter.SecretKey,
                    Username: parameter.Username,
                    Password: parameter.Password,
                    StartDate: parameter.StartTime.ToDateTimeOffset(),
                    EndDate: parameter.EndTime.ToDateTimeOffset());

                _logger.LogInformation("LogOn:{logOn} execute the command:{command}", DateTime.Now.ToString("o"), command);

                await _manualService
                    .ExecuteAsync(
                        command: command,
                        cancellationToken: cancellationToken)
                    .ConfigureAwait(false);

                var schedule3RdPartyPullRecordCommand = new Manual3rdPartyPullRecordCommand(
                    ApiUrl: new Uri(parameter.ApiUrl),
                    SecretKey: parameter.SecretKey,
                    Username: parameter.Username,
                    Password: parameter.Password,
                    StartDate: parameter.StartTime.ToDateTimeOffset(),
                    EndDate: parameter.EndTime.ToDateTimeOffset());

                _logger.LogInformation("LogOn:{logOn} execute the command:{command}", DateTime.Now.ToString("o"), command);

                await _manual3RdPartyService
                    .ExecuteAsync(
                        command: schedule3RdPartyPullRecordCommand,
                        cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
            }

            await _clinet
                .ScheduleFormCompleteAsync(
                    gameProviderCode: gameProviderCode,
                    id: id,
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An exception occurred in the game provider ({gameProviderCode}) schedule form. ({id})", gameProviderCode, id);

            await _clinet
                .ScheduleFormFaileAsync(
                    gameProviderCode: gameProviderCode,
                    id: id,
                    status: ex.Message,
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }
        finally
        {
            _ = await _scheduleFormLock
                 .UnlockAsync(
                     id: id)
                 .ConfigureAwait(false);
        }
    }
}
