using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Tgs.GameProviders.QueryServer.Models;

namespace Tgs.GameRecord.SkywindService.Core;

internal class UtensilMemoryCacheRepository : IUtensil
{
    private readonly TimeSpan _absoluteExpirationRelativeToNow;
    private readonly GameRecordInfoOptions _gameRecordInfo;
    private readonly IMemoryCache _memoryCache;
    private readonly IUtensil _utensil;

    public UtensilMemoryCacheRepository(
        IUtensil utensil,
        TimeSpan absoluteExpirationRelativeToNow,
        IOptions<GameRecordInfoOptions> options,
        IMemoryCache memoryCache)
    {
        _absoluteExpirationRelativeToNow = absoluteExpirationRelativeToNow;
        _gameRecordInfo = options.Value;
        _utensil = utensil ?? throw new ArgumentNullException(nameof(utensil));
        _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
    }

    public async ValueTask<Guid?> BrandGetGameProviderConfigurationIdAsync(Guid brandId, Guid gameProviderId)
    {
        var key = $"{_gameRecordInfo.GameProviderCode}:{brandId}:{gameProviderId}";
        if (!_memoryCache.TryGetValue(key, out Guid? gameProviderConfigurationId))
        {
            gameProviderConfigurationId = await _utensil.BrandGetGameProviderConfigurationIdAsync(brandId, gameProviderId).ConfigureAwait(false);

            if (gameProviderConfigurationId == null)
            {
                return null;
            }
            _ = _memoryCache.Set(key, gameProviderConfigurationId, absoluteExpirationRelativeToNow: _absoluteExpirationRelativeToNow);
        }
        return gameProviderConfigurationId;
    }

    public async ValueTask<int?> GetGameIdAsync(Guid gameProviderId, string gameExternalId)
    {
        var key = $"{_gameRecordInfo.GameProviderCode}:{gameExternalId}";
        if (!_memoryCache.TryGetValue(key, out int? gameId))
        {
            gameId = await _utensil.GetGameIdAsync(gameProviderId, gameExternalId).ConfigureAwait(false);

            if (gameId.HasValue)
            {
                _ = _memoryCache.Set(key, gameId, absoluteExpirationRelativeToNow: _absoluteExpirationRelativeToNow);
            }
        }
        return gameId;
    }

    public async ValueTask<Guid> GetGameProviderIdAsync(string gameProviderCode)
    {
        var key = $"{_gameRecordInfo.GameProviderCode}:{gameProviderCode}";
        if (!_memoryCache.TryGetValue(key, out Guid gameProviderId))
        {
            gameProviderId = await _utensil.GetGameProviderIdAsync(gameProviderCode).ConfigureAwait(false);

            _ = _memoryCache.Set(key, gameProviderId);
        }
        return gameProviderId;
    }

    public async ValueTask<PlayerModel?> GetPlayerAsync(string playerCode)
    {
        var key = $"{_gameRecordInfo.GameProviderCode}:{playerCode}";
        if (!_memoryCache.TryGetValue(key, out PlayerModel? playerModel))
        {
            playerModel = await _utensil.GetPlayerAsync(playerCode).ConfigureAwait(false);

            if (playerModel == null)
            {
                return null;
            }
            _ = _memoryCache.Set(key, playerModel, _absoluteExpirationRelativeToNow);
        }
        return playerModel;
    }
}
