using FastKartProject.DataAccessLayer;
using FastKartProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace FastKartProject.ViewComponents;

public class HeaderViewComponent : ViewComponent
{
    private readonly AppDbContext _dbContext;

    public HeaderViewComponent(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<ViewViewComponentResult> InvokeAsync()
    {
        var basketInString = Request.Cookies["basket"];

        if (string.IsNullOrEmpty(basketInString))
        {
            return View(new HeaderViewModel());
        }

        var basketViewModels = JsonConvert.DeserializeObject<List<BasketViewModel>>(basketInString);

        var newBasketViewModels = new List<BasketViewModel>();
        foreach (var product in basketViewModels)
        {
            var existProduct = _dbContext.Products.Find(product.ProductId);

            if (existProduct is null) continue;

            newBasketViewModels.Add(new BasketViewModel()
            {
                ProductId = existProduct.Id,
                ImageUrl = existProduct.ImageUrl,
                Price = existProduct.Price,
                Name = existProduct.Name,
                Count = product.Count
            });
        }

        double totalPrice = newBasketViewModels.Sum(x => x.Price * x.Count);
        int count = newBasketViewModels.Sum(x => x.Count);
        HeaderViewModel headerViewModel = new HeaderViewModel()
        {
            BasketViewModels = newBasketViewModels,
            TotalPrice = totalPrice,
            Count = count
        };

        return View(headerViewModel);

    }
}
