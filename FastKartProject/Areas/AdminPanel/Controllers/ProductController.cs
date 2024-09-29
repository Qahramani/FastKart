using FastKartProject.DataAccessLayer;
using FastKartProject.DataAccessLayer.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FastKartProject.Areas.AdminPanel.Controllers;

public class ProductController : AdminController
{
    private readonly AppDbContext _dbContext;

    public ProductController(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IActionResult> Index(Product product)
    {
        var products = await _dbContext.Products.Include(x => x.Category).ToListAsync();

        
        return View(products);
    }

    public IActionResult Create()
    {
        return View();
    }
}
