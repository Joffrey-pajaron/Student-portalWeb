using Dapper;
using StudentPortal.Web.Data;
using StudentPortal.Web.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StudentPortal.Web.Repositories
{
    public class InstructorRepository
    {
        private readonly DapperContext _context;

        public InstructorRepository(DapperContext context)
        {
            _context = context;
        }

        // Get all instructors
        public async Task<IEnumerable<Instructor>> GetAllAsync()
        {
            using var conn = _context.CreateConnection();
            string sql = "SELECT Id, FirstName, LastName, Email, Specialization FROM Instructors";
            return await conn.QueryAsync<Instructor>(sql);
        }

        // Get instructor by Id
        public async Task<Instructor?> GetByIdAsync(int id)
        {
            using var conn = _context.CreateConnection();
            string sql = "SELECT Id, FirstName, LastName, Email, Specialization FROM Instructors WHERE Id = @Id";
            return await conn.QueryFirstOrDefaultAsync<Instructor>(sql, new { Id = id });
        }

        // Add instructor (returns new instructor)
        public async Task<Instructor> AddAsync(Instructor instructor)
        {
            using var conn = _context.CreateConnection();
            string sql = @"
                INSERT INTO Instructors (FirstName, LastName, Email, Specialization)
                VALUES (@FirstName, @LastName, @Email, @Specialization);
                SELECT CAST(SCOPE_IDENTITY() as int);";

            int newId = await conn.ExecuteScalarAsync<int>(sql, instructor);
            instructor.Id = newId;
            return instructor;
        }

        // Update instructor
        public async Task<bool> UpdateAsync(Instructor instructor)
        {
            using var conn = _context.CreateConnection();
            string sql = @"
                UPDATE Instructors
                SET FirstName = @FirstName, LastName = @LastName, Email = @Email, Specialization = @Specialization
                WHERE Id = @Id";
            int rows = await conn.ExecuteAsync(sql, instructor);
            return rows > 0;
        }

        // Delete instructor
        public async Task<bool> DeleteAsync(int id)
        {
            using var conn = _context.CreateConnection();
            string sql = "DELETE FROM Instructors WHERE Id = @Id";
            int rows = await conn.ExecuteAsync(sql, new { Id = id });
            return rows > 0;
        }
    }
}
