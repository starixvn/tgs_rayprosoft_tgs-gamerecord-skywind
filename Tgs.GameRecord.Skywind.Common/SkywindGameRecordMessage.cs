using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Tgs.GameRecord.Skywind.Common;

public record SkywindGameRecordMessage
{
	[Required]
	[DataType(DataType.Url)]
	[DefaultValue(typeof(Uri), "https://api.prerelease-env.biz/")]
	public Uri ApiUrl { get; init; } = default!;
	[Required]
	[DefaultValue(typeof(string), "66a44892-54dc-488b-b755-1d1da4f8ad61")]
	public string SecretKey { get; init; } = default!;
	[Required]
	[DefaultValue(typeof(string), "PGT_brand_api_stg")]
	public string Username { get; init; } = default!;
	[Required]
	[DefaultValue(typeof(string), "ArSg5e6CR2g1wIgB")]
	public string Password { get; init; } = default!;
	[Required]
	[DataType(DataType.DateTime)]
	[DefaultValue(typeof(DateTimeOffset), "2023-01-04T12:00:00+08:00")]
	public DateTimeOffset StartTime { get; init; } = default!;
	[Required]
	[DataType(DataType.DateTime)]
	[DefaultValue(typeof(DateTimeOffset), "2023-01-04T13:00:08+00:00")]
	public DateTimeOffset EndTime { get; init; } = default!;
}
