using Microsoft.AspNetCore.Mvc;
using StudentPortal.Web.Services;

public class AccountController : Controller
{
    private readonly AuthService authService;

    // Constructor injection
    public AccountController(AuthService authService)
    {
        this.authService = authService;
    }

    public IActionResult Login() => View();

    [HttpPost]
    public IActionResult Login(string username, string password)
    {
        var user = authService.Login(username, password);
        if (user != null)
        {
            HttpContext.Session.SetInt32("UserId", user.UserId);
            HttpContext.Session.SetString("RoleName", user.RoleName);
            return RedirectToAction("Index", "Home");
        }
        ViewBag.Error = "Invalid username or password";
        return View();
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }

    public IActionResult Profile()
    {
        int? userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null) return RedirectToAction("Login");

        var profile = authService.GetProfile(userId.Value);
        return View(profile);
    }
}
