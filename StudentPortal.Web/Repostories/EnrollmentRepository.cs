using Dapper;
using Microsoft.Data.SqlClient;
using StudentPortal.Web.Data;
using StudentPortal.Web.Models.Entities;
using System.Data;

namespace StudentPortal.Web.Repositories
{
    public class EnrollmentRepository
    {
        private readonly DapperContext _context;

        public EnrollmentRepository(DapperContext context)
        {
            _context = context;
        }

        private IDbConnection CreateConnection() => _context.CreateConnection();

        public async Task<IEnumerable<Enrollment>> GetAllAsync()
        {
            using var conn = CreateConnection();
            string sql = @"
                SELECT e.Id, e.StudentId, e.CourseId, e.EnrollmentDate,
                       s.FirstName + ' ' + s.LastName AS StudentName,
                       c.CourseName
                FROM Enrollments e
                JOIN Students s ON e.StudentId = s.Id
                JOIN Courses c ON e.CourseId = c.Id";
            return await conn.QueryAsync<Enrollment>(sql);
        }

        public async Task<Enrollment?> GetByIdAsync(Guid id)
        {
            using var conn = CreateConnection();
            string sql = @"
                SELECT e.Id, e.StudentId, e.CourseId, e.EnrollmentDate,
                       s.FirstName + ' ' + s.LastName AS StudentName,
                       c.CourseName
                FROM Enrollments e
                JOIN Students s ON e.StudentId = s.Id
                JOIN Courses c ON e.CourseId = c.Id
                WHERE e.Id = @Id";
            return await conn.QueryFirstOrDefaultAsync<Enrollment>(sql, new { Id = id });
        }

        public async Task<Enrollment> AddAsync(Enrollment enrollment)
        {
            using var conn = CreateConnection();

            // 1️⃣ Insert enrollment, let SQL Server generate the Id (assuming Id is INT IDENTITY)
            string insertSql = @"
        INSERT INTO Enrollments (StudentId, CourseId, EnrollmentDate)
        VALUES (@StudentId, @CourseId, GETDATE());
        SELECT CAST(SCOPE_IDENTITY() AS int);"; // Returns the newly generated Id

            // 2️⃣ Execute insert and get new Id
            var newId = await conn.ExecuteScalarAsync<int>(insertSql, enrollment);
            enrollment.Id = newId; // assign it to object

            // 3️⃣ Retrieve the full enrollment info with student name and course name
            string selectSql = @"
        SELECT e.Id, e.StudentId, e.CourseId, e.EnrollmentDate,
               s.FirstName + ' ' + s.LastName AS StudentName,
               c.CourseName
        FROM Enrollments e
        JOIN Students s ON e.StudentId = s.Id
        JOIN Courses c ON e.CourseId = c.Id
        WHERE e.Id = @Id";

            return await conn.QuerySingleAsync<Enrollment>(selectSql, new { Id = newId });
        }


        public async Task<bool> DeleteAsync(Guid id)
        {
            using var conn = CreateConnection();
            string sql = "DELETE FROM Enrollments WHERE Id = @Id";
            int rows = await conn.ExecuteAsync(sql, new { Id = id });
            return rows > 0;
        }
    }
}
