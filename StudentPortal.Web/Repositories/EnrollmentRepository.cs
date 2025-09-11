using Dapper;
using StudentPortal.Web.Data;
using StudentPortal.Web.Models.Entities;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace StudentPortal.Web.Repositories
{
    public class EnrollmentRepository
    {
        private readonly DapperContext _context;

        public EnrollmentRepository(DapperContext context)
        {
            _context = context;
        }

        // Enroll student in a course
        public async Task EnrollStudentAsync(int studentId, int courseId)
        {
            var sql = @"INSERT INTO Enrollments (Id, StudentId, CourseId, EnrollmentDate)
                VALUES (@Id, @StudentId, @CourseId, GETDATE())";

            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(sql, new
                {
                    Id = Guid.NewGuid(),
                    StudentId = studentId,
                    CourseId = courseId
                });
            }
        }




        // Get all courses for a student
        // Get all courses for a student
        public async Task<IEnumerable<dynamic>> GetCoursesByStudentAsync(int studentId)
        {
            var sql = @"SELECT e.Id AS EnrollmentId, c.Id, c.CourseName, c.Description
                FROM Courses c
                INNER JOIN Enrollments e ON c.Id = e.CourseId
                WHERE e.StudentId = @StudentId";

            using (var connection = _context.CreateConnection())
            {
                return await connection.QueryAsync(sql, new { StudentId = studentId });
            }
        }

        // Drop enrollment (remove student from a course)
        public async Task<int> DropEnrollmentAsync(Guid enrollmentId)
        {
            var sql = @"DELETE FROM Enrollments WHERE Id = @EnrollmentId";
            using (var connection = _context.CreateConnection())
            {
                return await connection.ExecuteAsync(sql, new { EnrollmentId = enrollmentId });
            }
        }
    }
}
