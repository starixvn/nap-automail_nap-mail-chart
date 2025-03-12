using Fanex.Chart.Business.Model;
using OfficeOpenXml;
using System.Collections.Generic;
using System.IO;

namespace Fanex.Chart.Business.Services;

public interface ExcelService
{
    string Export(string group, List<ProductDTO> data, int reportIndex);

    int LastRowIndex { get; set; }
}

public class ExcelServiceImp(DateService dateService) : ExcelService
{
    public int LastRowIndex { get; set; }

    public string Export(string group, List<ProductDTO> data, int reportIndex)
    {
        using var excelPackage = new ExcelPackage(new FileInfo("Templates/Letstalk-DCWL.xlsx"));
        using var template = excelPackage.Workbook.Worksheets["DCWL"];
        const int startRowIndex = 5;
        LastRowIndex = data.Count + startRowIndex;

        template.Cells["B2"].Value = $"{dateService.WinlostDate:yyyyMMdd} - {group}";
        template.Cells["B5"].LoadFromCollection(data, false);
        template.Cells["H1"].Value = $"{template.Cells["H1"].Value}.{reportIndex}";
        for (var columnIndex = 2; columnIndex <= 8; columnIndex++)
        {
            template.Cells[startRowIndex, columnIndex, data.Count + startRowIndex - 1, columnIndex].StyleID = template.Cells[startRowIndex, columnIndex].StyleID;
        }

        for (var rowIndex = startRowIndex; rowIndex < data.Count + startRowIndex; rowIndex++)
        {
            template.Row(rowIndex).Height = 19.5d;
        }

        return SaveExcelFile(excelPackage, group);
    }

    private string SaveExcelFile(ExcelPackage excelPackage, string group)
    {
        var date = dateService.WinlostDate;
        var fileName = $"Chart_{group}_{date:yyyyMMdd}.xlsx";
        var path = $"{AppConfigService.AppConfig.Path}/{date:yyyyMMdd}";
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        path = $"{path}/{fileName}";
        excelPackage.SaveAs(new FileInfo(path), AppConfigService.AppConfig.Password);

        return path;
    }
}