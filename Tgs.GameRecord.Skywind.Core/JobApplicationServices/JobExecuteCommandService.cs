using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tgs.GameRecord.PragmaticPlay.Common;
using Tgs.ServiceUtility;
using Valhalla.MessageQueue;

namespace Tgs.GameRecord.Skywind.Core.JobApplicationServices;

internal class JobExecuteCommandService(
    ILogger<JobExecuteCommandService> logger,
    IOptions<GameRecordInfoOptions> options,
    IMessageSender messageSender) : ICommandService<JobExecuteCommand>
{
    private readonly double _intervalMinutes = options.Value.IntervalMinutesMax < options.Value.IntervalMinutes
        ? options.Value.IntervalMinutesMax
        : options.Value.IntervalMinutesMinimum > options.Value.IntervalMinutes
        ? options.Value.IntervalMinutesMinimum
        : options.Value.IntervalMinutes;

    public ValueTask ExecuteAsync(JobExecuteCommand command, CancellationToken cancellationToken = default)
    {
        var endTime = DateTimeOffset.ParseExact(DateTimeOffset.UtcNow.ToString(@"yyyy-MM-dd\THH:mm:00K"), @"yyyy-MM-dd\THH:mm:ssK", null, styles: System.Globalization.DateTimeStyles.RoundtripKind);
        var startTime = endTime.AddMinutes(_intervalMinutes * -1);

        var parameters = new SkywindGameRecordMessage
        {
            ApiUrl = command.ApiUrl,
            SecretKey = command.SecretKey,
            Username = command.Username,
            Password = command.Password,
            StartTime = startTime,
            EndTime = endTime
        };

        logger.LogDebug("LogOn:{logOn} Parameters:{parameters}", DateTime.Now.ToString("u"), parameters);

        return messageSender
            .PublishAsync(
            subject: SkywindNatsConst.SchedulePublish,
            data: Encoding.UTF8.GetBytes(JsonSerializer.Serialize(parameters)),
            cancellationToken: cancellationToken);
    }
}
