namespace Fanex.Chart.Business.Model;

public class ProductDTO
{
    public string ProductCode { get; set; }

    public decimal WinLossMobile { get; set; }

    public decimal WinLossDesktop { get; set; }

    public decimal WinLossTotal => WinLossMobile + WinLossDesktop;

    public decimal TOMobile { get; set; }

    public decimal TODesktop { get; set; }

    public decimal TOTotal => TOMobile + TODesktop;
}