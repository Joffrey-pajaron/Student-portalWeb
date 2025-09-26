using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentPortal.Web.Models.Entities;
using StudentPortal.Web.Repostories;
using System;
using System.Threading.Tasks;

namespace StudentPortal.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CoursesController : Controller
    {
        private readonly CourseRepository _repo;

        // Inject CourseRepository via DI
        public CoursesController(CourseRepository repo)
        {
            _repo = repo;
        }

        // Load the page
        public IActionResult Index()
        {
            return View();
        }

        // GET: All courses for DataTables
        [HttpGet]
        public async Task<JsonResult> GetCourses()
        {
            try
            {
                var courses = await _repo.GetAllAsync();
                return Json(new { success = 1, data = courses });
            }
            catch (Exception ex)
            {
                return Json(new { success = 0, message = ex.Message, data = new object[] { } });
            }
        }

        // POST: Create course
        [HttpPost]
        public async Task<JsonResult> Create([FromBody] Course course)
        {
            try
            {
                int newId = await _repo.AddCourseAsync(course);
                course.Id = newId;
                return Json(new { success = 1, data = course });
            }
            catch (Exception ex)
            {
                return Json(new { success = 0, message = ex.Message });
            }
        }

        // POST: Edit course
        [HttpPost]
        public async Task<JsonResult> Edit([FromBody] Course course)
        {
            try
            {
                bool updated = await _repo.UpdateCourseAsync(course);
                if (!updated)
                    return Json(new { success = 0, message = "Course not found." });

                return Json(new { success = 1, data = course });
            }
            catch (Exception ex)
            {
                return Json(new { success = 0, message = ex.Message });
            }
        }

        // POST: Delete course
        public class DeleteCourseRequest
        {
            public int Id { get; set; }
        }

        [HttpPost]
        public async Task<JsonResult> Delete([FromBody] DeleteCourseRequest data)
        {
            try
            {
                int id = data.Id;
                bool deleted = await _repo.DeleteCourseAsync(id);
                if (!deleted)
                    return Json(new { success = 0, message = "Course not found." });

                return Json(new { success = 1 });
            }
            catch (Exception ex)
            {
                return Json(new { success = 0, message = ex.Message });
            }
        }

        // GET: One course by id (for edit)
        [HttpGet]
        public async Task<JsonResult> GetCourse(int id)
        {
            try
            {
                var course = await _repo.GetCourseAsync(id);
                if (course == null)
                    return Json(new { success = 0, message = "Course not found." });

                return Json(new { success = 1, data = course });
            }
            catch (Exception ex)
            {
                return Json(new { success = 0, message = ex.Message });
            }
        }
    }
}
