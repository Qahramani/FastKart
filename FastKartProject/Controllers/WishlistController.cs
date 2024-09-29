using FastKartProject.DataAccessLayer;
using FastKartProject.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FastKartProject.Controllers
{
    public class WishlistController : Controller
    {
        private readonly AppDbContext _dbcontext;
        private IWishlistService _wishlisService;

        public WishlistController(AppDbContext dbcontext, IWishlistService wishlisService)
        {
            _dbcontext = dbcontext;
            _wishlisService = wishlisService;
        }

        public async Task<IActionResult> Index()
        {
            
            var newWishlistViewModels = await _wishlisService.GetWishlist();

            return View(newWishlistViewModels);
        }

        public async Task<IActionResult> AddToWishlist(int? id)
        {
            int result = await _wishlisService.AddToWishlist(id);

            if (result == 0)
                return BadRequest();

            return RedirectToAction(nameof(Index),"home");
        }
    }
}
