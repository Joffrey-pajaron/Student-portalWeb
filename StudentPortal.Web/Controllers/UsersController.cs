using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentPortal.Web.Helpers;
using StudentPortal.Web.Models.Entities;
using StudentPortal.Web.Repositories;

namespace StudentPortal.Web.Controllers
{
    [Authorize(Roles = "Admin")] // only admin can manage users
    public class UsersController : Controller
    {
        private readonly UserRepository _userRepo;

        public UsersController(UserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        // ================= LIST =================
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userRepo.GetAllAsync();
            return Json(new { data = users });
        }

        // ================= ADD =================
        // ✅ Add
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddUser(AppUser user, string? password)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!string.IsNullOrEmpty(password))
                user.PasswordHash = PasswordHelper.HashPassword(password);

            await _userRepo.AddAsync(user);
            return Json(new { success = true });
        }

        // ✅ Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(AppUser user, string? password)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingUser = await _userRepo.GetByIdAsync(user.UserId);
            if (existingUser == null) return NotFound();

            existingUser.FullName = user.FullName;
            existingUser.Email = user.Email;
            existingUser.RoleId = user.RoleId;
            existingUser.StudentId = user.StudentId;
            existingUser.InstructorId = user.InstructorId;

            if (!string.IsNullOrEmpty(password))
                existingUser.PasswordHash = PasswordHelper.HashPassword(password);

            bool updated = await _userRepo.UpdateAsync(existingUser);
            return Json(new { success = updated });
        }

        // ✅ Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(int id)
        {
            bool deleted = await _userRepo.DeleteAsync(id);
            return Json(new { success = deleted });
        }

        // ================= SEARCH STUDENTS =================
        [HttpGet]
        public async Task<IActionResult> SearchStudents(string q)
        {
            var students = await _userRepo.SearchStudentsAsync(q);
            return Json(students);
        }

        // ================= SEARCH INSTRUCTORS =================
        [HttpGet]
        public async Task<IActionResult> SearchInstructors(string q)
        {
            var instructors = await _userRepo.SearchInstructorsAsync(q);
            return Json(instructors);
        }
    }
}
