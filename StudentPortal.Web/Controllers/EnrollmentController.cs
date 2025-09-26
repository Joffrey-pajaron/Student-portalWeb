using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentPortal.Web.Repositories;
using StudentPortal.Web.Repostories;
using System.Threading.Tasks;

namespace StudentPortal.Web.Controllers
{
    [Authorize(Roles = "Admin,Student")]
    public class EnrollmentController : Controller
    {   

        private readonly EnrollmentRepository _enrollmentRepo;
        private readonly IStudentRepository _studentRepo;
        private readonly CourseRepository _courseRepo;

        public EnrollmentController(
            EnrollmentRepository enrollmentRepo,
            IStudentRepository studentRepo,
            CourseRepository courseRepo)
        {
            _enrollmentRepo = enrollmentRepo;
            _studentRepo = studentRepo;
            _courseRepo = courseRepo;
        }

        // Load the page
        public IActionResult Index()
        {
            return View();
        }

        // Search student by Id
        [HttpGet]
        public async Task<JsonResult> GetStudent(int id)
        {
            var student = await _studentRepo.GetByIdAsync(id);
            if (student == null)
                return Json(new { success = 0, message = "Student not found" });

            return Json(new { success = 1, data = student });
        }

        // Get student courses
        [HttpGet]
        public async Task<JsonResult> GetStudentCourses(int studentId)
        {
            try
            {
                var courses = await _enrollmentRepo.GetCoursesByStudentAsync(studentId);
                return Json(new { success = 1, data = courses });
            }
            catch (Exception ex)
            {
                return Json(new { success = 0, message = ex.Message });
            }
        }


        // Enroll student in a course
        [HttpPost]
        public async Task<JsonResult> Enroll([FromBody] EnrollRequest request)
        {
            if (request == null)
                return Json(new { success = 0, message = "Invalid request" });

            // studentId is now int, no conversion needed
            await _enrollmentRepo.EnrollStudentAsync(request.StudentId, request.CourseId);

            return Json(new { success = 1, message = "Student enrolled successfully" });
        }



        // Drop enrollment
        [HttpPost]
        public async Task<JsonResult> Drop([FromBody] DropRequest request)
        {
            if (request == null)
                return Json(new { success = 0, message = "Invalid request" });

            var rows = await _enrollmentRepo.DropEnrollmentAsync(request.EnrollmentId);
            return Json(new { success = rows > 0 });
        }





        public class EnrollRequest
        {
            public int StudentId { get; set; }
            public int CourseId { get; set; }
        }

        public class DropRequest
        {
            public int EnrollmentId { get; set; }
        }
    }
}
