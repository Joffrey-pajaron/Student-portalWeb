namespace StudentPortal.Web.Models.Entities
{
    public class AppUser
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string? FullName { get; set; }
        public string? Email { get; set; }

        public int RoleId { get; set; }
        public string? RoleName { get; set; }

        // 🔗 Links
        public int? StudentId { get; set; }
        public int? InstructorId { get; set; }
    }
}
