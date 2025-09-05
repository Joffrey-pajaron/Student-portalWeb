using StudentPortal.Web.Models.Entities;

public class Enrollment
{
    public int Id { get; set; } // auto-increment
    public int StudentId { get; set; } // integer FK
    public int CourseId { get; set; } // keep int or GUID depending on your course setup
    public DateTime EnrollmentDate { get; set; }

    public Student Student { get; set; }
    public Course Course { get; set; }
}
