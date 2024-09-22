using FastKartProject.Models;
using Microsoft.AspNetCore.Mvc;

namespace FastKartProject.Services.Interfaces;

public interface IBasketService
{
    Task<List<BasketViewModel>> GetBasket();
    Task<(int,string)> AddToBasket(int? id);

}
