using Dapper;
using StudentPortal.Web.Data;
using StudentPortal.Web.Models.Entities;
using System.Data;

namespace StudentPortal.Web.Repositories
{
    public class InstructorRepository
    {
        private readonly DapperContext _context;

        public InstructorRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Instructor>> GetAllAsync()
        {
            using var conn = _context.CreateConnection();
            return await conn.QueryAsync<Instructor>(
                "SELECT Id, FirstName, LastName, Email, Specialization FROM Instructors"
            );
        }

        public async Task<Instructor?> GetByIdAsync(Guid id)
        {
            using var conn = _context.CreateConnection();
            return await conn.QueryFirstOrDefaultAsync<Instructor>(
                "SELECT Id, FirstName, LastName, Email, Specialization FROM Instructors WHERE Id = @Id",
                new { Id = id }
            );
        }

        public async Task<Instructor> AddAsync(Instructor instructor)
        {
            instructor.Id = Guid.NewGuid(); // assuming your table uses uniqueidentifier

            using var conn = _context.CreateConnection();
            string sql = @"
                INSERT INTO Instructors (Id, FirstName, LastName, Email, Specialization)
                VALUES (@Id, @FirstName, @LastName, @Email, @Specialization);
                SELECT * FROM Instructors WHERE Id = @Id;"; // return the inserted instructor

            return await conn.QuerySingleAsync<Instructor>(sql, instructor);
        }

        public async Task<bool> UpdateAsync(Instructor instructor)
        {
            using var conn = _context.CreateConnection();
            string sql = @"
                UPDATE Instructors
                SET FirstName = @FirstName,
                    LastName = @LastName,
                    Email = @Email,
                    Specialization = @Specialization
                WHERE Id = @Id";

            int rows = await conn.ExecuteAsync(sql, instructor);
            return rows > 0;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            using var conn = _context.CreateConnection();
            string sql = "DELETE FROM Instructors WHERE Id = @Id";
            int rows = await conn.ExecuteAsync(sql, new { Id = id });
            return rows > 0;
        }
    }
}
