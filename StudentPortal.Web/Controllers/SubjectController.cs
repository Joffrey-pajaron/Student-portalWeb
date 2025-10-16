using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentPortal.Web.Models.Entities;
using StudentPortal.Web.Repositories;
using System;
using System.Threading.Tasks;

namespace StudentPortal.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SubjectsController : Controller
    {
        private readonly SubjectRepository _repo;

        public SubjectsController(SubjectRepository repo)
        {
            _repo = repo;
        }

        // Renders the subjects page
        public IActionResult Index() => View();

        [HttpGet]
        public async Task<JsonResult> SearchInstructors(string name)
        {
            try
            {
                var instructors = await _repo.SearchInstructorsByNameAsync(name);
                return Json(new { success = 1, data = instructors });
            }
            catch (Exception ex)
            {
                return Json(new { success = 0, message = ex.Message });
            }
        }


        // Get all subjects
        [HttpGet]
        public async Task<JsonResult> GetSubjects()
        {
            try
            {
                var subjects = await _repo.GetAllWithInstructorAsync();
                return Json(new { success = 1, data = subjects });
            }
            catch (Exception ex)
            {
                return Json(new { success = 0, message = ex.Message });
            }
        }

        // Add new subject
        [HttpPost]
        public async Task<JsonResult> Create([FromBody] Subject subject)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(subject.SubjectName))
                    return Json(new { success = 0, message = "Subject name is required" });

                int newId = await _repo.AddAsync(subject);
                subject.SubjectId = newId;
                return Json(new { success = 1, data = subject });
            }
            catch (Exception ex)
            {
                return Json(new { success = 0, message = ex.Message });
            }
        }

        // Optional: Delete
        [HttpPost]
        public async Task<JsonResult> Delete([FromBody] DeleteSubjectRequest data)
        {
            try
            {
                bool deleted = await _repo.DeleteAsync(data.SubjectId);
                if (!deleted) return Json(new { success = 0, message = "Subject not found" });
                return Json(new { success = 1, message = "Subject deleted" });
            }
            catch (Exception ex)
            {
                return Json(new { success = 0, message = ex.Message });
            }
        }

        public class DeleteSubjectRequest
        {
            public int SubjectId { get; set; }
        }
    }
}
