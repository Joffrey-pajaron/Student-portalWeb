using Dapper;
using StudentPortal.Web.Data;
using StudentPortal.Web.Models.Entities;
using System.Data;

namespace StudentPortal.Web.Repositories
{
    public class StudentRepository : IStudentRepository
    {
        private readonly DapperContext _context;

        public StudentRepository(DapperContext context)
        {
            _context = context;
        }

        private IDbConnection CreateConnection() => _context.CreateConnection();

        public async Task<IEnumerable<Student>> GetAllAsync()
        {
            using var conn = CreateConnection();
            return await conn.QueryAsync<Student>(
                "SELECT Id, FirstName, LastName, Email FROM Students"
            );
        }

        public async Task<Student?> GetByIdAsync(int id)
        {
            using var conn = CreateConnection();
            return await conn.QueryFirstOrDefaultAsync<Student>(
                "SELECT Id, FirstName, LastName, Email FROM Students WHERE Id = @Id",
                new { Id = id }
            );
        }

        public async Task<Student> AddAsync(Student student)
        {
            using var connection = _context.CreateConnection();

            string sql = @"
                INSERT INTO Students (FirstName, LastName, Email) 
                VALUES (@FirstName, @LastName, @Email);
                SELECT CAST(SCOPE_IDENTITY() as int);
            ";

            var id = await connection.QuerySingleAsync<int>(sql, student);
            student.Id = id; // assign auto-generated identity
            return student;
        }

        public async Task<bool> UpdateAsync(Student student)
        {
            using var conn = CreateConnection();
            string sql = @"
                UPDATE Students
                SET FirstName = @FirstName,
                    LastName = @LastName,
                    Email = @Email
                WHERE Id = @Id;
            ";

            int rows = await conn.ExecuteAsync(sql, student);
            return rows > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using var conn = CreateConnection();
            string sql = "DELETE FROM Students WHERE Id = @Id";
            int rows = await conn.ExecuteAsync(sql, new { Id = id });
            return rows > 0;
        }
    }
}
