using Dapper;
using StudentPortal.Web.Data;
using StudentPortal.Web.Models.Entities;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace StudentPortal.Web.Repostories
{
    public class CourseRepository
    {
        private readonly DapperContext _context;

        public CourseRepository(DapperContext context)
        {
            _context = context;
        }

        private IDbConnection CreateConnection() => _context.CreateConnection();

        public async Task<IEnumerable<Course>> GetAllAsync()
        {
            using var conn = CreateConnection();
            string sql = "SELECT Id, CourseName, Description FROM Courses";
            return await conn.QueryAsync<Course>(sql);
        }

        public async Task<Course> GetCourseAsync(int id)
        {
            using var conn = CreateConnection();
            string sql = "SELECT Id, CourseName, Description FROM Courses WHERE Id = @Id";
            return await conn.QueryFirstOrDefaultAsync<Course>(sql, new { Id = id });
        }

        public async Task<int> AddCourseAsync(Course course)
        {
            using var conn = CreateConnection();
            string sql = @"INSERT INTO Courses (CourseName, Description) 
                           VALUES (@CourseName, @Description);
                           SELECT CAST(SCOPE_IDENTITY() AS int);";
            return await conn.ExecuteScalarAsync<int>(sql, course);
        }

        public async Task<bool> UpdateCourseAsync(Course course)
        {
            using var conn = CreateConnection();
            string sql = "UPDATE Courses SET CourseName = @CourseName, Description = @Description WHERE Id = @Id";
            int rows = await conn.ExecuteAsync(sql, course);
            return rows > 0;
        }

        public async Task<bool> DeleteCourseAsync(int id)
        {
            using var conn = CreateConnection();
            string sql = "DELETE FROM Courses WHERE Id = @Id";
            int rows = await conn.ExecuteAsync(sql, new { Id = id });
            return rows > 0;
        }
    }
}
