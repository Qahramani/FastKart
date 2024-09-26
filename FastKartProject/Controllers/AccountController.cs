using Microsoft.AspNetCore.Mvc;

namespace FastKartProject.Controllers;

public class AccountController : Controller
{
    public IActionResult Register()
    {
        return View();
    }
}
