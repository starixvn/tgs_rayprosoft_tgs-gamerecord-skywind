using Tgs.GameProviders.QueryServer.Models;

namespace Tgs.GameRecord.SkywindService.Core;

public interface IUtensil
{
    ValueTask<Guid?> BrandGetGameProviderConfigurationIdAsync(Guid brandId, Guid gameProviderId);

    ValueTask<int?> GetGameIdAsync(Guid gameProviderId, string gameExternalId);

    ValueTask<Guid> GetGameProviderIdAsync(string gameProviderCode);

    ValueTask<PlayerModel?> GetPlayerAsync(string playerCode);
}
