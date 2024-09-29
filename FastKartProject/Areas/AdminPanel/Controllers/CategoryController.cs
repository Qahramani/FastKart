using FastKartProject.DataAccessLayer;
using FastKartProject.DataAccessLayer.Entities;
using FastKartProject.Extentions;
using FastKartProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace FastKartProject.Areas.AdminPanel.Controllers;

public class CategoryController : AdminController
{
    private readonly AppDbContext _dbContext;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly string CATEGORY_IMAGES_PATH = "";

    public CategoryController(AppDbContext dbContext, IWebHostEnvironment webHostEnvironment)
    {
        _dbContext = dbContext;
        _webHostEnvironment = webHostEnvironment;
        CATEGORY_IMAGES_PATH = Path.Combine(_webHostEnvironment.WebRootPath, "assets", "svg", "fashion");
    }

    public async Task<IActionResult> Index()
    {
        var categoryList = await _dbContext.Categories.ToListAsync();

        return View(categoryList);
    }


    public async Task<IActionResult> Details(int? id)
    {
        if (id is null) return NotFound();

        var category = await _dbContext.Categories.FindAsync(id);

        if (category is null) return NotFound();

        return View(category);
    }
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View();
        }
        if (!model.ImageFile.CheckType("image"))
        {
            ModelState.AddModelError("ImageFile", "Please use image format");
            return View();
        }

        if (!model.ImageFile.CheckSize(2))
        {
            ModelState.AddModelError("ImageFile", "Image size should be <= 2mb");
            return View();
        }

        var isExist = await _dbContext.Categories.AnyAsync(x => x.Name.ToLower() == model.Name.ToLower());

        if (isExist)
        {
            ModelState.AddModelError("Name", "Category with given name already exist");
            return View();
        }

        var imageUrl = await model.ImageFile.GenerateFileAsync(CATEGORY_IMAGES_PATH);

        var newCategory = new Category()
        {
            Name = model.Name,
            Description = model.Description,
            ImageUrl = imageUrl
        };

        await _dbContext.Categories.AddAsync(newCategory);

        await _dbContext.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Update(int? id)
    {
        if (id == null) return NotFound();

        var category = await _dbContext.Categories.FindAsync(id);

        if (category is null)  return NotFound();

        var model = new UpdateCategoryViewModel()
        {
            Name = category.Name,
            Description = category.Description,
            ImageFile = category.ImageFile,
            Id = category.Id
        };
        return View(model);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(UpdateCategoryViewModel model)
    {
        var existingCategory = await _dbContext.Categories.FindAsync(model.Id);

        if (existingCategory == null) return NotFound();

        if (!ModelState.IsValid)
        {
            View();
        }
        var isExist = await _dbContext.Categories.AnyAsync(x => x.Name.ToLower() == model.Name.ToLower() && x.Id != model.Id);

        if (isExist)
        {
            ModelState.AddModelError("Name", "Category with given name already exist");
            return View();
        }


        if (model.ImageFile != null)
        {
            if (!model.ImageFile.CheckType("image"))
            {
                ModelState.AddModelError("ImageFile", "Please use image format");
                return View();
            }
            if (!model.ImageFile.CheckSize(2))
            {
                ModelState.AddModelError("ImageFile", "Image size should be <= 2mb");
                return View();
            }
            var path = Path.Combine(CATEGORY_IMAGES_PATH, existingCategory.ImageUrl);

            if (System.IO.File.Exists(path))
                System.IO.File.Delete(path);

            var newImageUrl = await model.ImageFile.GenerateFileAsync(CATEGORY_IMAGES_PATH);
            existingCategory.ImageUrl = newImageUrl;
        }

        existingCategory.Name = model.Name;
        existingCategory.Description = model.Description;



        _dbContext.Categories.Update(existingCategory);
        await _dbContext.SaveChangesAsync();


        return RedirectToAction(nameof(Index));

    }

   
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var existingCategory = await _dbContext.Categories.FindAsync(id);

        if (existingCategory == null) return NotFound();

        var path = Path.Combine(CATEGORY_IMAGES_PATH, existingCategory.ImageUrl);
        if (System.IO.File.Exists(path))
            System.IO.File.Delete(path);

        _dbContext.Categories.Remove(existingCategory);
        await _dbContext.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}
