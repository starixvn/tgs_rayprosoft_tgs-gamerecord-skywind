namespace Tgs.GameRecord.SkywindService.ViewModels;

public record GameRecordQueryViewModel(
    Uri GameApiUrl,
    string SecretKey,
    string Username,
    string Password,
    DateTimeOffset StartDate,
    DateTimeOffset EndDate);
