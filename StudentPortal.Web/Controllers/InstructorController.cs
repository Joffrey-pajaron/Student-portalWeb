using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentPortal.Web.Models.Entities;
using StudentPortal.Web.Repositories;

namespace StudentPortal.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class InstructorController : Controller
    {
        private readonly InstructorRepository _repo;

        public InstructorController(InstructorRepository repo)
        {
            _repo = repo;
        }

        // Renders the index page
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        // ✅ Fetch all instructors (used by DataTables)
        [HttpGet]
        public async Task<IActionResult> GetInstructors()
        {
            try
            {
                var instructors = await _repo.GetAllAsync();
                return Json(new { data = instructors }); // DataTables format
            }
            catch (Exception ex)
            {
                // Log to console (shows up in VS Output window)
                Console.WriteLine("❌ ERROR in GetInstructors: " + ex.Message);

                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }


        // ✅ Fetch single instructor by Id
        [HttpGet]
        public async Task<IActionResult> GetInstructor(int id)
        {
            if (id <= 0)
                return BadRequest(new { success = false, message = "Invalid instructor ID" });

            var instructor = await _repo.GetByIdAsync(id);
            if (instructor == null)
                return NotFound(new { success = false, message = "Instructor not found" });

            return Ok(new { success = true, data = instructor });
        }

        // ✅ Create or update an instructor
        [HttpPost]
        public async Task<IActionResult> SaveInstructor([FromBody] Instructor instructor)
        {
            if (instructor == null)
                return BadRequest(new { success = false, message = "Invalid instructor data" });

            try
            {
                if (instructor.Id == 0) // Create (new instructor, no Id yet)
                {
                    var newInstructor = await _repo.AddAsync(instructor);
                    return Ok(new { success = true, data = newInstructor });
                }
                else // Update
                {
                    var existing = await _repo.GetByIdAsync(instructor.Id);
                    if (existing == null)
                        return NotFound(new { success = false, message = "Instructor not found" });

                    await _repo.UpdateAsync(instructor);
                    return Ok(new { success = true, data = instructor });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Server error: {ex.Message}" });
            }
        }

        // ✅ Delete instructor
        [HttpPost]
        public async Task<IActionResult> DeleteInstructor([FromBody] DeleteInstructorRequest data)
        {
            if (data.Id <= 0)
                return BadRequest(new { success = false, message = "Invalid instructor ID" });

            var deleted = await _repo.DeleteAsync(data.Id);
            if (!deleted)
                return NotFound(new { success = false, message = "Instructor not found" });

            return Ok(new { success = true, message = "Instructor deleted successfully" });
        }

        // DTO for delete request
        public class DeleteInstructorRequest
        {
            public int Id { get; set; }
        }
    }
}
