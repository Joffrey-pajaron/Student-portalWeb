using Microsoft.AspNetCore.Mvc;

namespace StudentPortal.Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View(); // This will load Views/Home/Index.cshtml
        }
    }
}
