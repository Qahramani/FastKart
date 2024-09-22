using Microsoft.AspNetCore.Mvc;

namespace FastKartProject.Areas.AdminPanel.Controllers;

public class DashboardController : AdminController
{
    public IActionResult Index()
    {
        return View();
    }
}
