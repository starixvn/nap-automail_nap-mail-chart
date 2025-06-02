using Fanex.Chart.Business.Services;
using Fanex.MailDelivery.Client.Jobs;
using Fanex.MailDelivery.Core.Models;
using Quartz;

namespace Fanex.Chart.Service.Services;

public class ExecuteService : JobBase
{
    protected override JobMessage Run(Schedule schedule, IJobExecutionContext jobContext)
    {
        var services = ServiceCollectionExtensions.Services;
        var serviceProvider = services.BuildServiceProvider();

        var mainService = serviceProvider.GetService<MainService>();
        if (mainService == null)
        {
            return new JobMessage();
        }

        var result = mainService.Execute();
        AppConfigService.AppConfig.Done = true;
        return new JobMessage
        {
            JobStatus = result.JobStatus,
            Schedule = schedule,
            Mail = result.Mail
        };
    }
}