using System.Text.Json;
using Microsoft.Extensions.Logging;
using Tgs.GameRecord.Skywind.Core.ApplicationServices;
using Tgs.GameRounds.Schedule.Client;
using Tgs.GameRounds.Schedule.Common.Protos;
using Tgs.GameRounds.Schedule.Protos;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.Skywind.Core;

internal class ScheduleFormDispatcher : IScheduleFormDispatcher
{
    private readonly IGameRoundsScheduleClient _clinet;
    private readonly ILogger<ScheduleFormDispatcher> _logger;
    private readonly ICommandService<PullRecordBy3rdPartyCommand> _pulllRecord3rdPartyService;
    private readonly IScheduleFormLock _scheduleFormLock;
    private readonly ICommandService<PullRecordCommand> _service;

    public ScheduleFormDispatcher(
        ILogger<ScheduleFormDispatcher> logger,
        IGameRoundsScheduleClient clinet,
        IScheduleFormLock scheduleFormLock,
        ICommandService<PullRecordCommand> service,
        ICommandService<PullRecordBy3rdPartyCommand> pulllRecord3rdPartyService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _clinet = clinet ?? throw new ArgumentNullException(nameof(clinet));
        _scheduleFormLock = scheduleFormLock ?? throw new ArgumentNullException(nameof(scheduleFormLock));
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _pulllRecord3rdPartyService = pulllRecord3rdPartyService ?? throw new ArgumentNullException(nameof(pulllRecord3rdPartyService));
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

            try
            {
                var command = new PullRecordCommand(
                    ApiUrl: new Uri(parameter.ApiUrl),
                    SecretKey: parameter.SecretKey,
                    Username: parameter.Username,
                    Password: parameter.Password,
                    StartDate: parameter.StartTime.ToDateTimeOffset(),
                    EndDate: parameter.EndTime.ToDateTimeOffset());

                _logger.LogInformation("LogOn:{logOn} execute the command:{command}", DateTime.Now.ToString("o"), command);

                await _service
                    .ExecuteAsync(
                        command: command,
                        cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "LogOn:{logOn}  An exception occurred in the process of game account pulling.", DateTime.Now.ToString("o"));
            }

            try
            {
                var command = new PullRecordBy3rdPartyCommand(
                    ApiUrl: new Uri(parameter.ApiUrl),
                    SecretKey: parameter.SecretKey,
                    Username: parameter.Username,
                    Password: parameter.Password,
                    StartDate: parameter.StartTime.ToDateTimeOffset(),
                    EndDate: parameter.EndTime.ToDateTimeOffset());

                _logger.LogInformation("LogOn:{logOn} execute the command:{command}", DateTime.Now.ToString("o"), command);

                await _pulllRecord3rdPartyService
                    .ExecuteAsync(
                        command: command,
                        cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "LogOn:{logOn}  An exception occurred in the 3rdParty game account opening process.", DateTime.Now.ToString("o"));
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
