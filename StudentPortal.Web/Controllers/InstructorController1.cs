using Microsoft.AspNetCore.Mvc;
using StudentPortal.Web.Models.Entities;
using StudentPortal.Web.Repositories;

namespace StudentPortal.Web.Controllers
{
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

        // Fetch all instructors (used by DataTables)
        [HttpGet]
        public async Task<IActionResult> GetInstructors()
        {
            var instructors = await _repo.GetAllAsync();
            return Json(new { data = instructors });
        }

        // Fetch single instructor by Id
        [HttpGet]
        public async Task<IActionResult> GetInstructor(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest(new { success = false, message = "Invalid instructor ID" });

            var instructor = await _repo.GetByIdAsync(id);
            if (instructor == null)
                return NotFound(new { success = false, message = "Instructor not found" });

            return Ok(new { success = true, data = instructor });
        }

        // Create or update an instructor
        [HttpPost]
        public async Task<IActionResult> SaveInstructor([FromBody] Instructor instructor)
        {
            if (instructor == null)
                return BadRequest(new { success = false, message = "Invalid instructor data" });

            try
            {
                if (instructor.Id == Guid.Empty) // Create
                {
                    instructor.Id = Guid.NewGuid();
                    await _repo.AddAsync(instructor);
                }
                else // Update
                {
                    var existing = await _repo.GetByIdAsync(instructor.Id);
                    if (existing == null)
                        return NotFound(new { success = false, message = "Instructor not found" });

                    await _repo.UpdateAsync(instructor);
                }

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Server error: {ex.Message}" });
            }
        }

        // Delete instructor
        [HttpPost]
        public async Task<IActionResult> DeleteInstructor([FromBody] DeleteInstructorRequest data)
        {
            if (data.Id == Guid.Empty)
                return BadRequest(new { success = false, message = "Invalid instructor ID" });

            var deleted = await _repo.DeleteAsync(data.Id);
            if (!deleted)
                return NotFound(new { success = false, message = "Instructor not found" });

            return Ok(new { success = true, message = "Instructor deleted successfully" });
        }

        public class DeleteInstructorRequest
        {
            public Guid Id { get; set; }
        }
    }
    }
