using Dapper;
using StudentPortal.Web.Data;
using StudentPortal.Web.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StudentPortal.Web.Repositories
{
    // DTO class to include instructor name
    public class SubjectWithInstructor
    {
        public int SubjectId { get; set; }
        public string SubjectName { get; set; } = "";
        public int InstructorId { get; set; }
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string FullName => $"{FirstName} {LastName}";
    }

    public class SubjectRepository
    {
        private readonly DapperContext _context;

        public SubjectRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<dynamic>> SearchInstructorsByNameAsync(string name)
        {
            var sql = @"
        SELECT TOP 10 Id, FirstName, LastName
        FROM Instructors
        WHERE FirstName LIKE @Search OR LastName LIKE @Search";

            using var conn = _context.CreateConnection();
            return await conn.QueryAsync(sql, new { Search = "%" + name + "%" });
        }

        // ✅ Get all subjects with instructor names
        public async Task<IEnumerable<SubjectWithInstructor>> GetAllWithInstructorAsync()
        {
            var sql = @"
                SELECT s.SubjectId, s.SubjectName, s.InstructorId,
                       i.FirstName, i.LastName
                FROM Subjects s
                LEFT JOIN Instructors i ON s.InstructorId = i.Id";

            using var conn = _context.CreateConnection();
            return await conn.QueryAsync<SubjectWithInstructor>(sql);
        }

        // ✅ Get single subject with instructor name
        public async Task<SubjectWithInstructor> GetByIdWithInstructorAsync(int id)
        {
            var sql = @"
                SELECT s.SubjectId, s.SubjectName, s.InstructorId,
                       i.FirstName, i.LastName
                FROM Subjects s
                LEFT JOIN Instructors i ON s.InstructorId = i.Id
                WHERE s.SubjectId = @Id";

            using var conn = _context.CreateConnection();
            return await conn.QueryFirstOrDefaultAsync<SubjectWithInstructor>(sql, new { Id = id });
        }

        // ✅ Add new subject
        public async Task<int> AddAsync(Subject subject)
        {
            var sql = @"
                INSERT INTO Subjects (SubjectName, InstructorId)
                VALUES (@SubjectName, @InstructorId);
                SELECT CAST(SCOPE_IDENTITY() as int);";

            using var conn = _context.CreateConnection();
            return await conn.ExecuteScalarAsync<int>(sql, subject);
        }

        // ✅ Update subject
        public async Task<bool> UpdateAsync(Subject subject)
        {
            var sql = @"
                UPDATE Subjects
                SET SubjectName = @SubjectName, InstructorId = @InstructorId
                WHERE SubjectId = @SubjectId";

            using var conn = _context.CreateConnection();
            int rows = await conn.ExecuteAsync(sql, subject);
            return rows > 0;
        }

        // ✅ Delete subject
        public async Task<bool> DeleteAsync(int id)
        {
            var sql = "DELETE FROM Subjects WHERE SubjectId = @Id";
            using var conn = _context.CreateConnection();
            int rows = await conn.ExecuteAsync(sql, new { Id = id });
            return rows > 0;
        }
    }
}
