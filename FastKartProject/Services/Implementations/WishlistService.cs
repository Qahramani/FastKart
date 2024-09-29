using FastKartProject.DataAccessLayer;
using FastKartProject.Models;
using FastKartProject.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace FastKartProject.Services.Implementations;

public class WishlistService : IWishlistService
{
    private readonly AppDbContext _dbContext;
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly string COOKIE_BASKET_KEY = "wishlist";

    public WishlistService(AppDbContext dbContext, IHttpContextAccessor contextAccessor)
    {
        _dbContext = dbContext;
        _contextAccessor = contextAccessor;
    }

    public async Task<int> AddToWishlist(int? id)
    {
        var product = await _dbContext.Products.FindAsync(id);

        var wishlistViewModels = new List<WishlistViewModel>();

        if (product is null)
            return 0;

        var basket = _contextAccessor.HttpContext.Request.Cookies[COOKIE_BASKET_KEY];

        if (basket is not null)
            wishlistViewModels = JsonConvert.DeserializeObject<List<WishlistViewModel>>(basket);

        var existProduct = wishlistViewModels.FirstOrDefault(x => x.ProductId == id);

        if (existProduct is not null)
            existProduct.Count++;
        else
        {
            wishlistViewModels.Add(new WishlistViewModel
            {
                ProductId = product.Id,
                Name = product.Name,
                Price = product.Price,
                ImageUrl = product.ImageUrl,
                Count = 1
            });
        }

        var json = JsonConvert.SerializeObject(wishlistViewModels);

        _contextAccessor.HttpContext.Response.Cookies.Append(COOKIE_BASKET_KEY, json);

        return 1;
    }

    public async Task<List<WishlistViewModel>> GetWishlist()
    {
        string? basket = _contextAccessor.HttpContext?.Request.Cookies[COOKIE_BASKET_KEY];

        var wishlistViewModels = new List<WishlistViewModel>();

        if (basket is not null)
            wishlistViewModels = JsonConvert.DeserializeObject<List<WishlistViewModel>>(basket);

        var newWishlistViewModels = new List<WishlistViewModel>();

        foreach (var product in wishlistViewModels)
        {
            var existProduct = await _dbContext.Products.FindAsync(product.ProductId);

            if (existProduct is null) continue;

            newWishlistViewModels.Add(new WishlistViewModel()
            {
                ProductId = existProduct.Id,
                ImageUrl = existProduct.ImageUrl,
                Price = existProduct.Price,
                Name = existProduct.Name,
                Count = product.Count
            });
        }

        return newWishlistViewModels;

    }
}
