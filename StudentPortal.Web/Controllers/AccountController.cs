using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentPortal.Web.Helpers;
using StudentPortal.Web.Models.Entities;
using StudentPortal.Web.Repositories;
using System.Security.Claims;

namespace StudentPortal.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserRepository _userRepo;

        public AccountController(UserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        // ================= LOGIN =================
        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            var user = await _userRepo.GetByUsernameAsync(username);

            if (user == null || !PasswordHelper.VerifyPassword(user.PasswordHash, password))
            {
                ViewBag.Error = "Invalid username or password";
                return View();
            }

            // ✅ Convert RoleId into role name
            string roleName = user.RoleId switch
            {
                1 => "Admin",
                2 => "Teacher",
                3 => "Student",
                _ => "User"
            };


            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim("FullName", user.FullName ?? string.Empty),
                new Claim(ClaimTypes.Role, roleName)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return RedirectToAction("Index", "Home");
        }

        // ================= REGISTER =================
        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(string username, string email, string fullname, string password, int roleId, int? studentId, int? instructorId)
        {
            var user = new AppUser
            {
                Username = username,
                Email = email,
                FullName = fullname,
                RoleId = roleId,
                StudentId = roleId == 3 ? studentId : null,        // ✅ link to Student table if role is Student
                InstructorId = roleId == 2 ? instructorId : null,  // ✅ link to Instructor table if role is Instructor
                PasswordHash = PasswordHelper.HashPassword(password)
            };

            await _userRepo.AddAsync(user);
            return RedirectToAction("Login");
        }

        // ================= LOGOUT =================
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
