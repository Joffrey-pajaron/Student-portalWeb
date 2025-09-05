using Microsoft.AspNetCore.Mvc;
using StudentPortal.Web.Models.Entities;
using StudentPortal.Web.Repositories;
using StudentPortal.Web.Repostories;

namespace StudentPortal.Web.Controllers
{
    public class EnrollmentController : Controller
    {
        private readonly IStudentRepository _studentRepo;
        private readonly CourseRepository _courseRepo;
        private readonly EnrollmentRepository _repo;

        public EnrollmentController(EnrollmentRepository repo, IStudentRepository studentRepo, CourseRepository courseRepo)
        {
            _repo = repo;
            _studentRepo = studentRepo;
            _courseRepo = courseRepo;
        }

        [HttpGet]
        public async Task<IActionResult> GetStudents()
        {
            var students = await _studentRepo.GetAllAsync();
            return Json(students);
        }

        [HttpGet]
        public async Task<IActionResult> GetCourses()
        {
            var courses = await _courseRepo.GetAllAsync();
            return Json(courses);
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetEnrollments()
        {
            var enrollments = await _repo.GetAllAsync();
            return Json(new { data = enrollments });
        }

        [HttpPost]
        public async Task<IActionResult> SaveEnrollment([FromBody] Enrollment enrollment)
        {
            if (enrollment == null)
                return BadRequest(new { success = false, message = "Invalid enrollment data" });

            try
            {
                await _repo.AddAsync(enrollment);
                return Ok(new { success = true, message = "Enrollment added successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }


        [HttpDelete]
        public async Task<IActionResult> DeleteEnrollment(Guid id)
        {
            var deleted = await _repo.DeleteAsync(id);
            if (!deleted)
                return Json(new { success = false, message = "Enrollment not found" });

            return Json(new { success = true, message = "Enrollment deleted successfully" });
        }
    }
}
