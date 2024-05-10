using Tgs.GameRecord.Skywind.Core.JobApplicationServices;

namespace Tgs.GameRecord.Skywind.Core;

public interface IHangfireUtensil
{
	public Task JobCheckAsync();

	public Task JobExecuteAsync(JobExecuteCommand source);

	public Task JobInitAsync();
}
