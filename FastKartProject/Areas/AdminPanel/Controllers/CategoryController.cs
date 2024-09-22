using FastKartProject.DataAccessLayer;
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

    public IActionResult Create()
    {
        return View();
    }

    public IActionResult Details(int? id)
    {
        return View();
    }
}
