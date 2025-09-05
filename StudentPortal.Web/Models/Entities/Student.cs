public class Student
{
    public int Id { get; set; } // auto-increment integer
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }

    // Optional navigation
    public ICollection<Enrollment>? Enrollments { get; set; }  // ✅ now optional
}
