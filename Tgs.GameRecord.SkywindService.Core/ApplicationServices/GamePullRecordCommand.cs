namespace Tgs.GameRecord.SkywindService.Core.ApplicationServices;

public record GamePullRecordCommand(IEnumerable<GameInterface.SkywindService.Protos.GameRecord> Records);
