using Fanex.Chart.Business.Model;
using System.Collections.Generic;
using System.Linq;

namespace Fanex.Chart.Business.Services;

public interface TransformService
{
    List<ProductDTO> Execute(List<RawData> data);
}

public class TransformServiceImp : TransformService
{
    public List<ProductDTO> Execute(List<RawData> data)
    {
        const decimal zero = 0;
        var result = new List<ProductDTO>();
        data.GroupBy(g => new { g.ProductName })
            .ToList()
            .ForEach(productGroup =>
            {
                var rows = productGroup.ToList();
                var winLossMobile = rows.FirstOrDefault(s => s.IndicatorName.ToLowerInvariant().Equals("winloss") && s.IndicatorKey.ToLowerInvariant().Equals("mobile"));
                var winLossDesktop = rows.FirstOrDefault(s => s.IndicatorName.ToLowerInvariant().Equals("winloss") && s.IndicatorKey.ToLowerInvariant().Equals("desktop"));
                var toMobile = rows.FirstOrDefault(s => s.IndicatorName.ToLowerInvariant().Equals("turnover") && s.IndicatorKey.ToLowerInvariant().Equals("mobile"));
                var toDesktop = rows.FirstOrDefault(s => s.IndicatorName.ToLowerInvariant().Equals("turnover") && s.IndicatorKey.ToLowerInvariant().Equals("desktop"));

                var product = new ProductDTO
                {
                    ProductCode = rows[0].ProductCode,
                    WinLossMobile = winLossMobile?.IndicatorValue ?? zero,
                    WinLossDesktop = winLossDesktop?.IndicatorValue ?? zero,
                    TOMobile = toMobile?.IndicatorValue ?? zero,
                    TODesktop = toDesktop?.IndicatorValue ?? zero
                };

                result.Add(product);
            });

        return result;
    }
}