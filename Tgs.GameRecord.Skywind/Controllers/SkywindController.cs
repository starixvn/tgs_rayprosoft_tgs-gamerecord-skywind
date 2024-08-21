using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Valhalla.MessageQueue;
using Tgs.GameRecord.Skywind.Common;

namespace Tgs.GameRecord.Skywind.Controllers;

[Route("[controller]")]
[ApiController]
public class SkywindController : ControllerBase
{
	/// <summary>
	/// Games the record manual asynchronous.
	/// </summary>
	/// <param name="messageSender">The message sender.</param>
	/// <param name="source">The source.</param>
	/// <returns></returns>
	[HttpPost("manual")]
	public ValueTask GameRecordManualAsync(
		[FromServices] IMessageSender messageSender,
		SkywindGameRecordMessage source)
		=> messageSender.PublishAsync(
			subject: SkywindNatsConst.ManualPublish,
			data: JsonSerializer.SerializeToUtf8Bytes(source),
			cancellationToken: HttpContext.RequestAborted);

	/// <summary>
	/// Games the record schedule asynchronous.
	/// </summary>
	/// <param name="messageSender">The message sender.</param>
	/// <param name="source">The source.</param>
	/// <returns></returns>
	[HttpPost("schedule")]
	public ValueTask GameRecordScheduleAsync(
		[FromServices] IMessageSender messageSender,
		SkywindGameRecordMessage source)
		=> messageSender.PublishAsync(
			subject: SkywindNatsConst.SchedulePublish,
			data: JsonSerializer.SerializeToUtf8Bytes(source),
			cancellationToken: HttpContext.RequestAborted);
}
