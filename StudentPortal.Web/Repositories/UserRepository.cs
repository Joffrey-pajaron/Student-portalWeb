using Dapper;
using Microsoft.Data.SqlClient;
using StudentPortal.Web.Models.Entities;
using System.Data;

namespace StudentPortal.Web.Repositories
{
    public class UserRepository
    {
        private readonly string _connectionString;

        public UserRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        private IDbConnection CreateConnection() => new SqlConnection(_connectionString);

        // ================= USERS =================
        public async Task<IEnumerable<AppUser>> GetAllAsync()
        {
            using var db = CreateConnection();
            string sql = @"
                SELECT u.UserId, u.Username, u.FullName, u.Email, 
                       u.RoleId, r.RoleName, u.StudentId, u.InstructorId
                FROM Users u
                INNER JOIN Roles r ON u.RoleId = r.RoleId";
            return await db.QueryAsync<AppUser>(sql);
        }

        public async Task<AppUser?> GetByIdAsync(int id)
        {
            using var db = CreateConnection();
            string sql = "SELECT * FROM Users WHERE UserId = @Id";
            return await db.QueryFirstOrDefaultAsync<AppUser>(sql, new { Id = id });
        }

        public async Task<AppUser?> GetByUsernameAsync(string username)
        {
            using var db = CreateConnection();
            string sql = "SELECT * FROM Users WHERE Username = @Username";
            return await db.QueryFirstOrDefaultAsync<AppUser>(sql, new { Username = username });
        }

        public async Task AddAsync(AppUser user)
        {
            using var db = CreateConnection();
            string sql = @"
                INSERT INTO Users (Username, PasswordHash, FullName, Email, RoleId, StudentId, InstructorId)
                VALUES (@Username, @PasswordHash, @FullName, @Email, @RoleId, @StudentId, @InstructorId)";
            await db.ExecuteAsync(sql, user);
        }

        public async Task<bool> UpdateAsync(AppUser user)
        {
            using var db = CreateConnection();
            string sql = @"
                UPDATE Users 
                SET FullName = @FullName, Email = @Email, RoleId = @RoleId,
                    StudentId = @StudentId, InstructorId = @InstructorId, PasswordHash = @PasswordHash
                WHERE UserId = @UserId";
            var rows = await db.ExecuteAsync(sql, user);
            return rows > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using var db = CreateConnection();
            string sql = "DELETE FROM Users WHERE UserId = @Id";
            var rows = await db.ExecuteAsync(sql, new { Id = id });
            return rows > 0;
        }

        // ================= SEARCH STUDENTS =================
        public async Task<IEnumerable<dynamic>> SearchStudentsAsync(string query)
        {
            using var db = CreateConnection();
            string sql = @"
                SELECT TOP 10 Id, (FirstName + ' ' + LastName) AS FullName, Email
                FROM Students
                WHERE FirstName LIKE @Search OR LastName LIKE @Search";
            return await db.QueryAsync(sql, new { Search = $"%{query}%" });
        }

        // ================= SEARCH INSTRUCTORS =================
        public async Task<IEnumerable<dynamic>> SearchInstructorsAsync(string query)
        {
            using var db = CreateConnection();
            string sql = @"
                SELECT TOP 10 Id, (FirstName + ' ' + LastName) AS FullName, Email
                FROM Instructors
                WHERE FirstName LIKE @Search OR LastName LIKE @Search";
            return await db.QueryAsync(sql, new { Search = $"%{query}%" });
        }
    }
}
