using StudentPortal.Web.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StudentPortal.Web.Repositories
{
    public interface IStudentRepository
    {
        Task<IEnumerable<Student>> GetAllAsync();
        Task<Student?> GetByIdAsync(int id);

        // Return the added student
        Task<Student> AddAsync(Student student);

        Task<bool> UpdateAsync(Student student);

        // Changed to int for consistency
        Task<bool> DeleteAsync(int id);
    }
}
