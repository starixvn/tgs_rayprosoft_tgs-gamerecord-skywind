using Microsoft.AspNetCore.Mvc;
using Tgs.GameRecord.SkywindService.Core.WorkerServices;
using Tgs.GameRecord.SkywindService.ViewModels;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.SkywindService.Controllers;

[ApiController]
[Route("evotf")]
public class EvolutionTransferController : ControllerBase
{
    [HttpPost]
    public ValueTask ExecuteAsync(
        [FromServices] ICommandService<SkywindSettleWorkerCommand> service,
        GameRecordQueryViewModel model)
        => service.ExecuteAsync(new SkywindSettleWorkerCommand(
            GameApiUrl: model.GameApiUrl,
            SecretKey: model.SecretKey,
            Username: model.Username,
            Password: model.Password,
            StartDate: model.StartDate,
            EndDate: model.EndDate));
}
