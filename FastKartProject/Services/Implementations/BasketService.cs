using FastKartProject.DataAccessLayer.Entities;
using FastKartProject.Models;
using FastKartProject.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace FastKartProject.Services.Implementations;

public class BasketService : IBasketService
{
    private readonly AppDbContext _dbContext;
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly string COOKIE_BASKET_KEY = "basket";

    public BasketService(AppDbContext dbContext, IHttpContextAccessor contextAccessor)
    {
        _dbContext = dbContext;
        _contextAccessor = contextAccessor;
    }

    public async Task<(int, string)> AddToBasket(int? id)
    {
        var product = await _dbContext.Products.FindAsync(id);

        var basketViewModels = new List<BasketViewModel>();

        if (product is null)
            return (0, "product is null");

        var basket = _contextAccessor.HttpContext.Request.Cookies[COOKIE_BASKET_KEY];

        if (basket is not null)
            basketViewModels = JsonConvert.DeserializeObject<List<BasketViewModel>>(basket);

        var existProduct = basketViewModels.FirstOrDefault(x => x.ProductId == id);

        if (existProduct is not null)
            existProduct.Count++;
        else
        {
            basketViewModels.Add(new BasketViewModel
            {
                ProductId = product.Id,
                Name = product.Name,
                Price = product.Price,
                ImageUrl = product.ImageUrl,
                Count = 1
            });
        }

        var json = JsonConvert.SerializeObject(basketViewModels);

        _contextAccessor.HttpContext.Response.Cookies.Append(COOKIE_BASKET_KEY, json);
        return (1,"product is added!");
    }

    public async Task<List<BasketViewModel>> GetBasket()
    {
        string? basket = _contextAccessor.HttpContext?.Request.Cookies[COOKIE_BASKET_KEY];

        var basketViewModels = new List<BasketViewModel>();

        if (basket is not null)
            basketViewModels = JsonConvert.DeserializeObject<List<BasketViewModel>>(basket);

        var newBasketViewModels = new List<BasketViewModel>();

        foreach (var product in basketViewModels)
        {
            var existProduct = await _dbContext.Products.FindAsync(product.ProductId);

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

        return newBasketViewModels;

    }
}
