using Microsoft.AspNetCore.Mvc;
using StudentPortal.Web.Repositories;
using System;
using System.Threading.Tasks;

namespace StudentPortal.Web.Controllers
{
    public class GradesController : Controller
    {
        private readonly GradesRepository _gradesRepo;

        public GradesController(GradesRepository gradesRepo)
        {
            _gradesRepo = gradesRepo;
        }

        // Load Grades main page
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Student()
        {
            return View();
        }

        public IActionResult Teacher()
        {
            return View();
        }



        // ✅ Student: Get all grades
        [HttpGet]
        public async Task<JsonResult> GetStudentGrades(int studentId)
        {
            try
            {
                var grades = await _gradesRepo.GetGradesByStudentAsync(studentId);
                return Json(new { success = 1, data = grades });
            }
            catch (Exception ex)
            {
                return Json(new { success = 0, message = ex.Message });
            }
        }

        // ✅ Teacher: Get subjects handled by teacher
        [HttpGet]
        public async Task<JsonResult> GetSubjectsByTeacher(int teacherId)
        {
            try
            {
                var subjects = await _gradesRepo.GetSubjectsByTeacherAsync(teacherId);
                return Json(new { success = 1, data = subjects });
            }
            catch (Exception ex)
            {
                return Json(new { success = 0, message = ex.Message });
            }
        }

        // ✅ Teacher: Get students enrolled in subject (with grades)
        [HttpGet]
        public async Task<JsonResult> GetStudentsBySubject(int subjectId)
        {
            try
            {
                var students = await _gradesRepo.GetStudentsBySubjectAsync(subjectId);
                return Json(new { success = 1, data = students });
            }
            catch (Exception ex)
            {
                return Json(new { success = 0, message = ex.Message });
            }
        }

        // ✅ Teacher: Save (insert or update) grade
        [HttpPost]
        public async Task<JsonResult> SaveGrade([FromBody] SaveGradeRequest request)
        {
            if (request == null)
                return Json(new { success = 0, message = "Invalid request" });

            var rows = await _gradesRepo.SaveGradeAsync(request.EnrollmentId, request.Grade);
            return Json(new { success = rows > 0 ? 1 : 0 });
        }

        // --- Old endpoints (optional if you prefer separate Add/Update) ---
        [HttpPost]
        public async Task<JsonResult> AddGrade([FromBody] AddGradeRequest request)
        {
            if (request == null)
                return Json(new { success = 0, message = "Invalid request" });

            var rows = await _gradesRepo.AddGradeAsync(request.EnrollmentId, request.SubjectId, request.Grade);
            return Json(new { success = rows > 0 ? 1 : 0 });
        }

        [HttpPost]
        public async Task<JsonResult> UpdateGrade([FromBody] UpdateGradeRequest request)
        {
            if (request == null)
                return Json(new { success = 0, message = "Invalid request" });

            var rows = await _gradesRepo.UpdateGradeAsync(request.Id, request.Grade);
            return Json(new { success = rows > 0 ? 1 : 0 });
        }

        // Request DTOs
        public class AddGradeRequest
        {
            public int EnrollmentId { get; set; }
            public int SubjectId { get; set; }
            public decimal Grade { get; set; }
        }

        public class UpdateGradeRequest
        {
            public int Id { get; set; }
            public decimal Grade { get; set; }
        }

        public class SaveGradeRequest
        {
            public int EnrollmentId { get; set; }
            public decimal Grade { get; set; }
        }
    }
}
