using Fanex.Chart.Business.Model;
using Fanex.Chart.Business.Services;
using Fanex.Data.Repository;
using Fanex.LetstalkService.Client;
using Fanex.MailDelivery.Client;
using Newtonsoft.Json;
using OfficeOpenXml;
using AppConfigService = Fanex.Chart.Business.Services.AppConfigService;
using LetstalkConfig = Fanex.LetstalkService.Client.AppConfigService;

namespace Fanex.Chart.Service;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection Services { get; set; }

    public static void AddConfiguration(this IServiceCollection services)
    {
        Services = services;

        Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

        var text = File.ReadAllText("appsettings.json");
        AppConfigService.AppConfig = JsonConvert.DeserializeObject<ConfigSettingModel>(text);
        LetstalkConfig.Config.LetstalkServiceApi = AppConfigService.AppConfig!.LetstalkServiceApi;
    }

    public static void AddInternalService(this IServiceCollection services)
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        services.AddSingleton<Log, LogImp>();
        services.AddSingleton<MainService, MainServiceImp>();
        services.AddSingleton<MessageService, MessageServiceImp>();
        services.AddSingleton<LetstalkClientService, LetstalkClientServiceImp>();
        services.AddSingleton<DataService, DataServiceImp>();
        services.AddSingleton<IDynamicRepository, DynamicRepository>();
        services.AddSingleton<ExcelService, ExcelServiceImp>();
        services.AddSingleton<DateService, DateServiceImp>();
        services.AddSingleton<ImageService, ImageServiceImp>();
        services.AddSingleton<TransformService, TransformServiceImp>();
        services.AddSingleton<FileService, FileServiceImp>();
        services.AddSingleton<ChartService, ChartServiceImp>();

        MailClientHandler.Start();
    }
}