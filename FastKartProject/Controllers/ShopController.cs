using Microsoft.AspNetCore.Mvc;

namespace FastKartProject.Controllers
{
    public class ShopController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Details(int? id)
        {
            ViewData["id"]=id;
            return View();
        }
    }
}
