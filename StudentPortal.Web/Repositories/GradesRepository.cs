using Dapper;
using StudentPortal.Web.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StudentPortal.Web.Repositories
{
    public class GradesRepository
    {
        private readonly DapperContext _context;

        public GradesRepository(DapperContext context)
        {
            _context = context;
        }

        // ✅ Get all grades for a student
        public async Task<IEnumerable<dynamic>> GetGradesByStudentAsync(int studentId)
        {
            var sql = @"
                SELECT g.Id AS GradeId, s.SubjectName, g.Grade, g.GradeDate
                FROM Grades g
                INNER JOIN Subjects s ON g.SubjectId = s.SubjectId
                INNER JOIN Enrollments e ON g.EnrollmentId = e.Id
                WHERE e.StudentId = @StudentId";

            using (var connection = _context.CreateConnection())
            {
                return await connection.QueryAsync(sql, new { StudentId = studentId });
            }
        }

        // ✅ Teacher: Get subjects handled by a teacher
        public async Task<IEnumerable<dynamic>> GetSubjectsByTeacherAsync(int teacherId)
        {
            var sql = @"
                SELECT SubjectId, SubjectName
                FROM Subjects
                WHERE InstructorId = @TeacherId";

            using (var connection = _context.CreateConnection())
            {
                return await connection.QueryAsync(sql, new { TeacherId = teacherId });
            }
        }

        // ✅ Teacher: Get students enrolled in a subject (with grades if already set)
        public async Task<IEnumerable<dynamic>> GetStudentsBySubjectAsync(int subjectId)
        {
            var sql = @"
        SELECT e.Id AS EnrollmentId,
               st.FirstName,
               st.LastName,
               st.Email,
               g.Grade AS CurrentGrade
        FROM Enrollments e
        INNER JOIN Students st ON st.Id = e.StudentId
        INNER JOIN Subjects s ON s.CourseId = e.CourseId
        LEFT JOIN Grades g ON g.EnrollmentId = e.Id AND g.SubjectId = s.SubjectId
        WHERE s.SubjectId = @SubjectId";

            using (var connection = _context.CreateConnection())
            {
                return await connection.QueryAsync(sql, new { SubjectId = subjectId });
            }
        }


        // ✅ Save grade (Insert if not exists, else Update)
        public async Task<int> SaveGradeAsync(int enrollmentId, decimal grade)
        {
            var sql = @"
                IF EXISTS (SELECT 1 FROM Grades WHERE EnrollmentId = @EnrollmentId)
                BEGIN
                    UPDATE Grades SET Grade = @Grade, GradeDate = GETDATE()
                    WHERE EnrollmentId = @EnrollmentId;
                END
                ELSE
                BEGIN
                    INSERT INTO Grades (EnrollmentId, SubjectId, Grade, GradeDate)
                    SELECT @EnrollmentId, s.SubjectId, @Grade, GETDATE()
                    FROM Enrollments e
                    INNER JOIN Subjects s ON e.CourseId = s.CourseId
                    WHERE e.Id = @EnrollmentId;
                END";

            using (var connection = _context.CreateConnection())
            {
                return await connection.ExecuteAsync(sql, new { EnrollmentId = enrollmentId, Grade = grade });
            }
        }

        // (Optional) Keep existing Add/Update methods for compatibility
        public async Task<int> AddGradeAsync(int enrollmentId, int subjectId, decimal grade)
        {
            var sql = @"INSERT INTO Grades (EnrollmentId, SubjectId, Grade, GradeDate)
                        VALUES (@EnrollmentId, @SubjectId, @Grade, GETDATE())";

            using (var connection = _context.CreateConnection())
            {
                return await connection.ExecuteAsync(sql, new { EnrollmentId = enrollmentId, SubjectId = subjectId, Grade = grade });
            }
        }

        public async Task<int> UpdateGradeAsync(int gradeId, decimal grade)
        {
            var sql = @"UPDATE Grades SET Grade = @Grade, GradeDate = GETDATE()
                        WHERE Id = @GradeId";

            using (var connection = _context.CreateConnection())
            {
                return await connection.ExecuteAsync(sql, new { GradeId = gradeId, Grade = grade });
            }
        }
        public async Task<IEnumerable<dynamic>> GetAllGradesAsync()
        {
            var sql = @"
        SELECT g.Id AS GradeId, 
               st.FirstName + ' ' + st.LastName AS StudentName,
               s.SubjectName, 
               g.Grade, 
               g.GradeDate
        FROM Grades g
        INNER JOIN Enrollments e ON g.EnrollmentId = e.Id
        INNER JOIN Students st ON e.StudentId = st.Id
        INNER JOIN Subjects s ON g.SubjectId = s.SubjectId";

            using (var connection = _context.CreateConnection())
            {
                return await connection.QueryAsync(sql);
            }
        }

        public async Task<int> DeleteGradeAsync(int gradeId)
        {
            var sql = "DELETE FROM Grades WHERE Id = @GradeId";
            using (var connection = _context.CreateConnection())
            {
                return await connection.ExecuteAsync(sql, new { GradeId = gradeId });
            }
        }

    }
}
