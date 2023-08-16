using Tgs.GameInterface.SkywindService.Client;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceRegisterExtensions
{
    public static async Task<WebApplication> UseGameRecordRegister(this WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var client = scope.ServiceProvider.GetRequiredService<IGameInterfaceSkywindClient>();

            _ = await client
                .CheckingServiceAsync(
                machineName: Environment.MachineName)
                .ConfigureAwait(false);
        }

        return app;
    }
}
