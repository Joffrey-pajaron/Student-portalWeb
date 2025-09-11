using Dapper;
using Microsoft.Extensions.Configuration;
using StudentPortal.Web.Data;
using StudentPortal.Web.Models.Entities;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace StudentPortal.Web.Repositories
{
    public class CourseSubjectServiceRepository
    {


        private readonly DapperContext _context;

        public CourseSubjectServiceRepository(DapperContext context)
        {
            _context = context;
        }

        private IDbConnection CreateConnection() => _context.CreateConnection();

        // Get all subjects assigned to a course
        public async Task<List<Subject>> GetSubjectsByCourseAsync(int courseId)
        {
            using var connection = CreateConnection();
            string sql = @"
                SELECT s.SubjectId, s.SubjectName, s.InstructorId
                FROM Subjects s
                INNER JOIN CourseSubjects cs ON s.SubjectId = cs.SubjectId
                WHERE cs.CourseId = @CourseId";

            var subjects = await connection.QueryAsync<Subject>(sql, new { CourseId = courseId });
            return subjects.ToList();
        }

        // Get all subjects
        public async Task<List<Subject>> GetAllSubjectsAsync()
        {
            using var connection = CreateConnection();
            string sql = "SELECT SubjectId, SubjectName, InstructorId FROM Subjects";
            var subjects = await connection.QueryAsync<Subject>(sql);
            return subjects.ToList();
        }

        // Add subjects to a course
        public async Task AddSubjectsToCourseAsync(int courseId, List<int> subjectIds)
        {
            using var connection = CreateConnection();
            string sql = @"
                IF NOT EXISTS (SELECT 1 FROM CourseSubjects WHERE CourseId = @CourseId AND SubjectId = @SubjectId)
                INSERT INTO CourseSubjects (CourseId, SubjectId)
                VALUES (@CourseId, @SubjectId)";

            foreach (var subjectId in subjectIds)
            {
                await connection.ExecuteAsync(sql, new { CourseId = courseId, SubjectId = subjectId });
            }
        }

        // Remove a subject from a course
        public async Task<bool> RemoveSubjectFromCourseAsync(int courseId, int subjectId)
        {
            using var connection = CreateConnection();
            string sql = "DELETE FROM CourseSubjects WHERE CourseId = @CourseId AND SubjectId = @SubjectId";

            int rowsAffected = await connection.ExecuteAsync(sql, new { CourseId = courseId, SubjectId = subjectId });
            return rowsAffected > 0;
        }
    }
}
