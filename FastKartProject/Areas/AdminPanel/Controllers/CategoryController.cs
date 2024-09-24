using FastKartProject.DataAccessLayer;
using FastKartProject.DataAccessLayer.Entities;
using FastKartProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FastKartProject.Areas.AdminPanel.Controllers;

public class CategoryController : AdminController
{
    private readonly AppDbContext _dbContext;

    public CategoryController(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IActionResult> Index()
    {
        var categoryList = await  _dbContext.Categories.ToListAsync();

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
        if(!ModelState.IsValid)
        {
            return BadRequest();
        }

        var isExist = await _dbContext.Categories.AnyAsync(x => x.Name.ToLower() ==  model.Name.ToLower());

        if(isExist)
        {
            ModelState.AddModelError("Name", "Category with given name already exist");
            return View();
        }

        var newCategory = new Category()
        {
            Name = model.Name,
            Description = model.Description,
            ImageUrl = "1.jpg"
        };

        await _dbContext.Categories.AddAsync(newCategory);

        await _dbContext.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}
