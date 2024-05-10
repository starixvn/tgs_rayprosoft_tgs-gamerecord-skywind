using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Tgs.GameRecord.PragmaticPlay.Common;
using Valhalla.MessageQueue;

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
	/// <remarks>
	/// <b>Example</b><br />
	///
	///     Post
	///
	///         LiveDealer Example
	///         {
	///             "apiUrl": "https://api.prerelease-env.biz/",
	///             "secretKey": "66a44892-54dc-488b-b755-1d1da4f8ad61",
	///             "username": "PGT_brand_api_stg",
	///             "password": "ArSg5e6CR2g1wIgB",
	///             "startTime": "2023-01-04T12:00:00+08:00",
	///             "endTime": "2023-01-04T13:00:08+00:00"
	///         }
	/// </remarks>
	[HttpPost("manual")]
	public ValueTask GameRecordManualAsync(
		[FromServices] IMessageSender messageSender,
		SkywindGameRecordMessage source)
		=> messageSender.PublishAsync(
			subject: SkywindNatsConst.ManualPublish,
			data: Encoding.UTF8.GetBytes(JsonSerializer.Serialize(source)),
			cancellationToken: HttpContext.RequestAborted);

	/// <summary>
	/// Games the record schedule asynchronous.
	/// </summary>
	/// <param name="messageSender">The message sender.</param>
	/// <param name="source">The source.</param>
	/// <returns></returns>
	/// <remarks>
	/// <b>Example</b><br />
	///
	///     Post
	///
	///         LiveDealer Example
	///         {
	///             "apiUrl": "https://api.prerelease-env.biz/",
	///             "secretKey": "66a44892-54dc-488b-b755-1d1da4f8ad61",
	///             "username": "PGT_brand_api_stg",
	///             "password": "ArSg5e6CR2g1wIgB",
	///             "startTime": "2023-01-04T12:00:00+08:00",
	///             "endTime": "2023-01-04T13:00:08+00:00"
	///         }
	/// </remarks>
	[HttpPost("schedule")]
	public ValueTask GameRecordScheduleAsync(
		[FromServices] IMessageSender messageSender,
		SkywindGameRecordMessage source)
		=> messageSender.PublishAsync(
			subject: SkywindNatsConst.SchedulePublish,
			data: Encoding.UTF8.GetBytes(JsonSerializer.Serialize(source)),
			cancellationToken: HttpContext.RequestAborted);
}
