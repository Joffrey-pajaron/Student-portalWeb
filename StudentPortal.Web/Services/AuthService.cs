using StudentPortal.Web.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;

namespace StudentPortal.Web.Services
{
    public class AuthService
    {
        private readonly string connectionString;

        public AuthService(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentException("DefaultConnection string is missing in appsettings.json.");
        }

        public User Login(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                return null;

            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string sql = @"
                   SELECT 
                    u.UserId, 
                    u.Username, 
                    u.PasswordHash, 
                    u.FullName, 
                    u.RoleId, 
                    r.RoleName 
                    FROM Users u
                    LEFT JOIN Roles r ON u.RoleId = r.RoleId
                    WHERE u.Username = @Username";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Username", username);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string storedPassword = reader["PasswordHash"]?.ToString();

                            // ⚠️ Plain-text comparison (temporary only)
                            if (storedPassword != null && password == storedPassword)
                            {
                                return new User
                                {
                                    UserId = reader["UserId"] == DBNull.Value ? 0 : Convert.ToInt32(reader["UserId"]),
                                    Username = reader["Username"]?.ToString(),
                                    FullName = reader["FullName"]?.ToString(),
                                    RoleId = reader["RoleId"] == DBNull.Value ? 0 : Convert.ToInt32(reader["RoleId"]),
                                    RoleName = reader["RoleName"] == DBNull.Value ? null : reader["RoleName"].ToString()
                                };
                            }
                        }
                    }
                }
            }

            return null;
        }

        public User GetProfile(int userId)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string sql = @"
                SELECT 
                    u.UserId, 
                    u.Username, 
                    u.FullName, 
                    u.RoleId, 
                    r.RoleName
                FROM Users u
                LEFT JOIN Roles r ON u.RoleId = r.RoleId
                WHERE u.UserId = @UserId";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@UserId", userId);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new User
                            {
                                UserId = reader["UserId"] == DBNull.Value ? 0 : Convert.ToInt32(reader["UserId"]),
                                Username = reader["Username"]?.ToString(),
                                FullName = reader["FullName"]?.ToString(),
                                RoleId = reader["RoleId"] == DBNull.Value ? 0 : Convert.ToInt32(reader["RoleId"]),
                                RoleName = reader["RoleName"] == DBNull.Value ? null : reader["RoleName"].ToString()
                            };
                        }
                    }
                }
            }

            return null;
        }
    }
}
