using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace StudentPortal.Web.Controllers
{
    [Authorize(Roles = "Admin,Instructor,Student")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View(); // This will load Views/Home/Index.cshtml
        }
    }
}
