using Microsoft.AspNetCore.Mvc;
using Tgs.GameRecord.SkywindService.Core.WorkerServices;
using Tgs.GameRecord.SkywindService.ViewModels;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.SkywindService.Controllers;

[ApiController]
[Route("swtf")]
public class SkywindTransferController : ControllerBase
{
    /// <summary>
    /// Executes the asynchronous.
    /// </summary>
    /// <param name="service">The service.</param>
    /// <param name="model">The model.</param>
    /// <returns></returns>
    /// <remarks>
    /// <b>Example</b><br />
    ///
    ///     Post
    ///
    ///     {
    ///         "gameApiUrl": "https://api.gcpstg.m27613.com/",
    ///         "secretKey": "66a44892-54dc-488b-b755-1d1da4f8ad61",
    ///         "username": "PGT_brand_api_stg",
    ///         "password": "ArSg5e6CR2g1wIgB",
    ///         "startDate": "2022-08-25T11:00:00+08:00",
    ///         "endDate": "2022-08-25T12:00:00+08:00",
    ///     }
    /// </remarks>
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
