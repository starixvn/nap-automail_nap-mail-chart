using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace Fanex.Chart.Service.Controllers;

using Business.Services;
using Olympus.Diagnostic;

[ApiController]
[Route("[controller]")]
public class Controller : ControllerBase
{
    [HttpGet]
    [Route("")]
    public ActionResult Index()
    {
        if (AppConfigService.AppConfig.Done)
        {
            return Problem(title: "Job is done. Ready to restart",
                instance: Environment.MachineName,
                detail: "Job is done. Ready to restart",
                type: "Job is done. Ready to restart",
                statusCode: (int)HttpStatusCode.ServiceUnavailable
            );
        }

        return Ok(Environment.MachineName);
    }

    [HttpGet]
    [Route("gc")]
    public string Gc()
    {
        var gc = GarbageCollector.CollectExtremely();
        return gc;
    }
}