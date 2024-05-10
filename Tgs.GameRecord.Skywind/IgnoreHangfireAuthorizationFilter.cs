using Hangfire.Dashboard;

namespace Tgs.GameRecord.Skywind;

public class IgnoreHangfireAuthorizationFilter : IDashboardAuthorizationFilter
{
	public bool Authorize(DashboardContext context) => true;
}
