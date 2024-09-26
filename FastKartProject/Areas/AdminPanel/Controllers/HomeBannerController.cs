using FastKartProject.DataAccessLayer.Entities;
using FastKartProject.Extentions;
using Microsoft.AspNetCore.Mvc;

namespace FastKartProject.Areas.AdminPanel.Controllers
{
    public class HomeBannerController : AdminController
    {
        private readonly AppDbContext _dbContext;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public HomeBannerController(AppDbContext dbContext, IWebHostEnvironment webHostEnvironment)
        {
            _dbContext = dbContext;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            var homeBanner = _dbContext.HomeBanners.ToList().Last();
            return View(homeBanner);
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(HomeBanner model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (!model.ImageFile.IsImage())
            {
                ModelState.AddModelError("ImageFile", "Please add image format");

                return View(model);
            }

            if(!model.ImageFile.IsValidSize(2))
            {
                ModelState.AddModelError("ImageFile", "Images length should be less than 2gb");

                return View(model);
            }

            var path = Path.Combine(_webHostEnvironment.WebRootPath, "assets", "images", "fashion", "home-banner");
            var imageName = await model.ImageFile.GenerateFileAsync(path);

            model.ImageUrl = imageName;

            await _dbContext.HomeBanners.AddAsync(model);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


    }
}
