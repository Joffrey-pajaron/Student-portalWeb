using Microsoft.AspNetCore.Mvc;
using StudentPortal.Web.Models.Entities;
using StudentPortal.Web.Repositories;
using System.Threading.Tasks;

namespace StudentPortal.Web.Controllers
{
    public class StudentsController : Controller
    {
        private readonly IStudentRepository _repository;

        public StudentsController(IStudentRepository repository)
        {
            _repository = repository;
        }

        // List page
        [HttpGet]
        public IActionResult List()
        {
            return View();
        }

        // Get all students (for DataTable)
        [HttpGet]
        public async Task<IActionResult> GetStudents()
        {
            var students = await _repository.GetAllAsync();
            return Json(new { data = students });
        }

        // Get single student by Id (for Edit modal)
        [HttpGet]
        public async Task<IActionResult> GetStudent(int id)
        {
            var student = await _repository.GetByIdAsync(id);
            if (student == null)
                return Json(new { success = false, message = "Student not found" });

            return Json(new { success = true, data = student });
        }

        // Add student
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] Student student)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage)
                                              .ToList();
                return Json(new { success = false, message = "Invalid data", errors });
            }

            // Don't set student.Id here, SQL Server will auto-generate it
            var addedStudent = await _repository.AddAsync(student);

            return Json(new
            {
                success = true,
                message = "Student added successfully",
                data = addedStudent
            });
        }


        // Edit student
        [HttpPost]
        public async Task<IActionResult> Edit([FromBody] Student student)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage)
                                              .ToList();
                return Json(new { success = false, message = "Invalid data", errors });
            }

            var updated = await _repository.UpdateAsync(student);
            if (!updated)
                return Json(new { success = false, message = "Student not found" });

            return Json(new { success = true, message = "Student updated successfully" });
        }

        // Delete student
        [HttpPost]
        public async Task<IActionResult> Delete([FromBody] DeleteStudentRequest request)
        {
            var deleted = await _repository.DeleteAsync(request.Id);
            if (!deleted)
                return Json(new { success = false, message = "Student not found" });

            return Json(new { success = true, message = "Student deleted successfully" });
        }

        public class DeleteStudentRequest
        {
            public int Id { get; set; }
        }
    }
}
