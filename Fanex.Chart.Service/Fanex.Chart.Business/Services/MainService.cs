using Fanex.Chart.Business.Model;
using Fanex.MailDelivery.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;

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
        var rawData = repository.QueryRawData();
        var reportIndex = 1;
        rawData.GroupBy(g => new { g.ProductGroup })
            .ToList()
            .ForEach(group =>
            {
                var data = transformService.Execute(group.ToList());
                var excelPath = excelService.Export(group.Key.ProductGroup, data, reportIndex);
                reportIndex++;

                var lastRow = excelService.LastRowIndex;
                var image = AppConfigService.AppConfig.Image[group.Key.ProductGroup];
                image.LastRow = lastRow;
                image.CropHeight = lastRow * image.HeightPerRow + image.AdjustHeight;

                var imagePath = imageService.ExportImage(excelPath, dateService.WinlostDate, "DCWL", image);
                images.Add(imagePath);
            });

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