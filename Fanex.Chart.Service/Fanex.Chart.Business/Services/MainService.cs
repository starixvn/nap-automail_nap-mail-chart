using Fanex.Chart.Business.Model;
using Fanex.MailDelivery.Core.Models;
using System;
using System.Collections.Generic;

namespace Fanex.Chart.Business.Services;

public interface MainService
{
    JobMessage Execute();
}

public class MainServiceImp(
    DataService repository,
    Log logService,
    TransformService transformService,
    ExcelService excelService,
    ImageService imageService,
    MessageService messageService,
    FileService fileService,
    DateService dateService,
    ChartService chartService)
    : MainService
{
    public JobMessage Execute()
    {
        var result = new ExecuteResult();
        JobMessage jobMessage;

        try
        {
            result = SendNotification();
        }
        catch (Exception e)
        {
            result.Success = false;
            result.Message = e.Message;
            logService.Log(e);
        }
        finally
        {
            jobMessage = new JobMessage
            {
                JobStatus = result.Success ?
                    JobStatus.SuccessWithoutSendmail :
                    JobStatus.DataIsNotReady,
                Mail = new Mail(),
                TaskLog = new TaskLog
                {
                    LogTime = DateTime.Now,
                    Message = result.Message
                }
            };
        }

        return jobMessage;
    }

    private ExecuteResult SendNotification()
    {
        if (!repository.IsDataAvailable())
        {
            return new ExecuteResult { Success = false, Message = "DataIsNotReady, TaskID = 38" };
        }

        var images = new List<string>();

        var chartPath = chartService.Execute();
        if (!string.IsNullOrEmpty(chartPath))
        {
            images.Add(chartPath);
        }

        bool result;
        if (AppConfigService.AppConfig.IsSentDirectly)
        {
            result = messageService.Send(images);
        }
        else
        {
            fileService.CopyFiles(images);
            result = true;
        }

        return new ExecuteResult { Success = result };
    }
}