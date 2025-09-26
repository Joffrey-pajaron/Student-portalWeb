using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentPortal.Web.Models.Entities;
using StudentPortal.Web.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StudentPortal.Web.Controllers
{
    [Authorize(Roles = "Admin,Student")]
    public class CourseSubjectController : Controller
    {
        private readonly CourseSubjectServiceRepository _repository;

        public CourseSubjectController(CourseSubjectServiceRepository repository)
        {
            _repository = repository;
        }

        // Render the main view (MVC page)
        public IActionResult Index()
        {
            return View(); // Ensure you have Views/CourseSubject/Index.cshtml
        }

        // Get all subjects for a specific course (AJAX)
        [HttpGet]
        public async Task<JsonResult> GetSubjectsByCourse(int courseId)
        {
            var subjects = await _repository.GetSubjectsByCourseAsync(courseId);
            return Json(new { success = 1, data = subjects });
        }

        // Get all subjects (AJAX)
        [HttpGet]
        public async Task<JsonResult> GetAllSubjects()
        {
            var subjects = await _repository.GetAllSubjectsAsync();
            return Json(new { success = 1, data = subjects });
        }

        // Add multiple subjects to a course (AJAX POST)
        [HttpPost]
        public async Task<IActionResult> AddSubjectsToCourse([FromBody] AddSubjectsRequest request)
        {
            try
            {
                if (request.SubjectIds == null || request.SubjectIds.Count == 0)
                    return BadRequest(new { success = false, message = "No subjects selected." });

                await _repository.AddSubjectsToCourseAsync(request.CourseId, request.SubjectIds);
                return Ok(new { success = true, message = "Subjects added successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }


        // Remove a subject from a course (AJAX POST)
        [HttpPost]
        public async Task<JsonResult> RemoveSubjectFromCourse([FromBody] RemoveSubjectRequest request)
        {
            var success = await _repository.RemoveSubjectFromCourseAsync(request.CourseId, request.SubjectId);
            if (!success)
                return Json(new { success = 0, message = "Subject not found for this course." });

            return Json(new { success = 1, message = "Subject removed successfully" });
        }
    }

    // Request models
    public class AddSubjectsRequest
    {
        public int CourseId { get; set; }
        public List<int> SubjectIds { get; set; }
    }

    public class RemoveSubjectRequest
    {
        public int CourseId { get; set; }
        public int SubjectId { get; set; }
    }
}
