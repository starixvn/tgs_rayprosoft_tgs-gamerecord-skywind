namespace Tgs.GameRecord.SkywindService.Core.ApplicationServices;

public record GamePullRecordCommand(Guid GameProviderId, string GameProviderCode, IEnumerable<GameInterface.SkywindService.Protos.GameRecord> Records);
