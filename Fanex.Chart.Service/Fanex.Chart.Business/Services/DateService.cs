using System;

namespace Fanex.Chart.Business.Services;

public interface DateService
{
    DateTime WinlostDate { get; }
}

public class DateServiceImp : DateService
{
    public DateTime WinlostDate => AppConfigService.AppConfig.IsManual ? AppConfigService.AppConfig.WinlostDate : DateTime.Today.AddDays(-1);
}