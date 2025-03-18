using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.SKCharts;
using LiveChartsCore.SkiaSharpView.VisualElements;
using SkiaSharp;
using System.IO;

namespace Fanex.Chart.Business.Services;

public interface ChartService
{
    string Execute();
}

public class ChartServiceImp(Log log) : ChartService
{
    public string Execute()
    {
        // Sample data (based on chart)
        var sbValues = new[] { 4.7, 4.3, 2.5, 3.4, 2.4, 0.9, 1.7, 5.7, 2.4, 1.9, 1.9 }; // SB
        var othersValues = new[] { 0.1, 0.2, 0.1, 0.2, 0.1, 0.1, 0.2, 0.2, 0.1, 0.1, 0.1 }; // Others
        var totalValues = new[] { 4.8, 4.5, 2.6, 3.6, 2.5, 1.0, 1.9, 7.9, 2.5, 2.0, 2.0 }; // Total

        // Create series for the chart
        var series = new ISeries[]
        {
            new ColumnSeries<double>
            {
                Name = "SB",
                Values = sbValues,
                Fill = new SolidColorPaint(SKColors.Blue)
            },
            new ColumnSeries<double>
            {
                Name = "Others",
                Values = othersValues,
                Fill = new SolidColorPaint(SKColors.Orange)
            },
            new LineSeries<double>
            {
                Name = "Total",
                Values = totalValues,
                Stroke = new SolidColorPaint(SKColors.Black) { StrokeThickness = 2 },
                Fill = null
            }
        };

        // Create X and Y axes
        var xAxes = new Axis[]
        {
            new()
            {
                Labels = ["1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11"],
                Name = "Date"
            }
        };

        var yAxes = new Axis[]
        {
            new()
            {
                Labeler = value => value.ToString("N1") + "m",
                Name = "Value"
            }
        };

        // Create an SKCartesianChart to draw the chart
        using var bitmap = new SKBitmap(800, 600);
        using var canvas = new SKCanvas(bitmap);

        var chart = new SKCartesianChart
        {
            Series = series,
            XAxes = xAxes,
            YAxes = yAxes,
            Width = 800,
            Height = 600,
            Title = new LabelVisual
            {
                Text = "DHWL [Mar-2025]\nMTD (as of 2025-03-11): 38.32m [2.95%]\nMTD (as of 2024-03-11): 32.87m [2.20%]\nDIFF %: +16.58% [+34.31%]",
                TextSize = 14,
                Padding = new LiveChartsCore.Drawing.Padding(10)
            }
        };

        var path = $"{AppConfigService.AppConfig.Path}/chart.png";
        try
        {
            // Draw chart on canvas
            chart.DrawOnCanvas(canvas);

            // Draw chart on canvas
            chart.DrawOnCanvas(canvas);

            // Save bitmap as PNG file
            using var image = SKImage.FromBitmap(bitmap);
            using var data = image.Encode(SKEncodedImageFormat.Png, 100);

            using var stream = File.OpenWrite(path);
            data.SaveTo(stream);
            if (!File.Exists(path))
            {
                path = string.Empty;
            }
        }
        catch (System.Exception e)
        {
            log.Log(e);
        }

        return path;
    }
}