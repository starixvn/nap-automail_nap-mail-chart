using Fanex.Data.Repository;
using Fanex.Chart.Business.Criteria;
using Fanex.Chart.Business.Model;
using System.Collections.Generic;
using System.Linq;

namespace Fanex.Chart.Business.Services;

public interface DataService
{
    Dictionary<string, List<Product>> Query();

    bool IsDataAvailable();

    TaskStatusModel QueryStatus();

    List<RawData> QueryRawData();

    void LetstalkPreRunLogInsert(LogInternalOneTimeModel oneTimeModel);
}

public class DataServiceImp(IDynamicRepository repository, DateService dateService) : DataService
{
    private static Dictionary<string, string> TextNeedToReplace => new()
    {
        { "Live Casino", "LC" },
        { "RNG Casino", "R&G" },
        { "AG Casino", "AG" },
        { "Gold Deluxe", "GD" },
        { "Microgaming - Live Casino", "MGC-LC" },
        { "Allbet", "AB" },
        { "Microgaming - RNG Casino", "MGC-R&G" },
        { "Macau Games", "MAG" },
        { "Pragmatic", "PP" },
        { "Poker Games", "PokerG" },
        { "Arcadia Gaming", "Arcadia" },
        { "AE Sexy", "AE Sexy" },
        { "GDG", "GDG LC" },
        { "BG", "BG LC" },
        { "WM", "WM LC" },
        { "MaxGame", "MaxGame" },
        { "Joker", "Joker" },
        { "PP Live Casino", "PP LC" },
        { "Habanero", "Habanero" },
        { "CG", "CreativeG" },
        { "Vgaming", "Vgaming" },
        { "AdvantPlay", "AdPlay" },
        { "AdvantPlay Mini", "AdPlay Mini" },
        { "SABA.games", "SBG" },
        { "Saba Casino", "SabaCasino" },
        { "FGG", "FGG" },
        { "SEAL Entertainment", "SealEn" },
        { "Jili", "Jili" },
        { "YeeBet", "YeeBet" },
        { "WE Live Casino", "WE LC" },
        { "Live22", "Live22" },
        { "SABA xD", "SABAxD" },
        { "Yolo Play", "YoloP" },
        { "UU SLOTS", "UUS" },
        { "Funky Games", "FunkyG" },
        { "Virtual Games", "VirtualG" },
        { "FA CHAI", "FACHAI" },
        { "FastSpin", "FastSpin" },
        { "ON Casino", "ONCasino" },
        { "GPI Live Casino", "GPI LC" },
        { "SABA Keno", "SABAK" },
        { "Nextspin", "Nextspin" },
        { "Playtech", "Playtech" },
        { "GPI", "GPI" },
        { "PLAYSTAR", "PLAYS" },
        { "ASKMESLOT", "ASKME" }
    };

    public Dictionary<string, List<Product>> Query()
    {
        var allProducts = new Dictionary<string, List<Product>>();
        var rawData = QueryRawData();
        rawData.Select(s => s.ProductGroup)
            .Distinct()
            .ToList()
            .ForEach(productGroup =>
            {
                var rawDataByProductGroup = rawData.Where(s => s.ProductGroup == productGroup).ToList();
                var productByGroup = GroupFigureByProduct(rawDataByProductGroup);
                allProducts.Add(productGroup, productByGroup);
            });

        return allProducts;
    }

    public List<RawData> QueryRawData()
    {
        var criteria = new WinLossCriteria
        {
            WinlostDate = dateService.WinlostDate
        };
        var rawData = repository.Fetch<RawData>(criteria).ToList();
        return TransFormProduct(rawData);
    }

    private static List<RawData> TransFormProduct(List<RawData> rawData)
    {
        var texts = TextNeedToReplace;
        rawData.ForEach(row =>
        {
            var newProductGroup = texts.TryGetValue(row.ProductGroup, out var text) ? text : row.ProductGroup;
            var newProductCode = texts.TryGetValue(row.ProductName, out var text1) ? text1 : row.ProductCode;

            row.ProductGroup = newProductGroup;
            row.ProductCode = newProductCode;
        });

        return rawData;
    }

    private static List<Product> GroupFigureByProduct(List<RawData> rawData)
    {
        var products = rawData.GroupBy(g => new { g.ProductId, g.ProductName })
            .Select(p => new Product
            {
                ProductId = p.Key.ProductId,
                ProductName = p.Key.ProductName,
                Figures = p.Select(s => new Figure
                {
                    IndicatorKey = s.IndicatorKey,
                    IndicatorName = s.IndicatorName,
                    IndicatorValue = s.IndicatorValue
                }).ToList()
            }).ToList();

        return products;
    }

    public TaskStatusModel QueryStatus()
    {
        return repository.Get<TaskStatusModel>(new GetDWStatusCriteria());
    }

    public bool IsDataAvailable()
    {
        if (AppConfigService.AppConfig.IsManual || !AppConfigService.AppConfig.IsCheckEnoughData)
        {
            return true;
        }

        var result = QueryStatus().DataIsReady;

        return result;
    }

    public void LetstalkPreRunLogInsert(LogInternalOneTimeModel oneTimeModel)
    {
        var criteria = new LetstalkPreRunLogInsertCriteria
        {
            FolderName = oneTimeModel.FolderName,
            Date = oneTimeModel.Date,
            GenerateTime = oneTimeModel.GenerateTime,
            TargetName = oneTimeModel.TargetName,
            Group = oneTimeModel.Group,
            MailId = oneTimeModel.MailId,
            IsSent = false
        };

        repository.Execute(criteria);
    }
}