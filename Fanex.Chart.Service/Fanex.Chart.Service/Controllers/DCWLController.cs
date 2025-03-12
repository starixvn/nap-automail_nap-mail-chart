using Fanex.Chart.Business.Services;
using Fanex.MailDelivery.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace Fanex.Chart.Service.Controllers;

[ApiController]
[Route("[controller]")]
public class DCWLController(MainService service) : ControllerBase
{
    [HttpGet]
    public JobMessage Get()
    {
        var message = service.Execute();
        AppConfigService.AppConfig.Done = true;

        return message;
    }
}