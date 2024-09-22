namespace FastKartProject.Models;

public class HeaderViewModel
{
    public List<BasketViewModel> BasketViewModels { get; set; } = new List<BasketViewModel>();
    public double TotalPrice { get; set; }
    public int Count { get; set; }
}
