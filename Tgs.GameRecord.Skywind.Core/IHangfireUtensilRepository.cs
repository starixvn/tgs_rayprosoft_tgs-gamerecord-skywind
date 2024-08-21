using Tgs.GameRecord.Skywind.Core.JobApplicationServices;

namespace Tgs.GameRecord.Skywind.Core;

public interface IHangfireUtensilRepository
{
	public Task JobCheckAsync();

	public Task JobExecuteAsync(JobExecuteCommand source);

	public Task JobInitAsync();
}
