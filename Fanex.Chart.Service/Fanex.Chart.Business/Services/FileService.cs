using Fanex.Chart.Business.Model;
using System;
using System.Collections.Generic;
using System.IO;

namespace Fanex.Chart.Business.Services;

public interface FileService
{
    void CopyFiles(List<string> fileNames);
}

public class FileServiceImp(DataService dataService, DateService dateService) : FileService
{
    private void LogOneTime(DateTime date)
    {
        var fileInfo = new FileInfo(AppConfigService.AppConfig.InternalPath);
        var oneTimeModel = new LogInternalOneTimeModel
        {
            Date = $"{date:yyyyMMdd}",
            FolderName = fileInfo.Name,
            GenerateTime = $"{DateTime.Now:yyyy-MM-dd hh:mm:ss tt}",
            TargetName = AppConfigService.AppConfig.TargetName,
            Group = AppConfigService.AppConfig.Group
        };

        dataService.LetstalkPreRunLogInsert(oneTimeModel);
    }

    public void CopyFiles(List<string> fileNames)
    {
        var date = dateService.WinlostDate;
        var path = $"{AppConfigService.AppConfig.InternalPath}/{date:yyyyMMdd}";
        Directory.CreateDirectory(path);
        var index = 1;
        fileNames.ForEach(fileName =>
        {
            var sortName = $"{index}_{new FileInfo(fileName).Name}";
            var destinationPath = $"{path}/{sortName}";
            File.Copy(fileName, destinationPath, true);
            index++;
        });

        LogOneTime(date);
    }
}