using System.Collections.Generic;

namespace Fanex.Chart.Business.Model;

public class Product
{
    public string ProductName { get; set; }

    public int ProductId { get; set; }

    public List<Figure> Figures { get; set; } = [];
}