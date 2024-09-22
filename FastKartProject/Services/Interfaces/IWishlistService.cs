using FastKartProject.Models;

namespace FastKartProject.Services.Interfaces;

public interface IWishlistService
{
    Task<List<WishlistViewModel>> GetWishlist();
    Task<int> AddToWishlist(int? id);
}
