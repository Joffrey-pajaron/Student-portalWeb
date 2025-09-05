using StudentPortal.Web.Models.Entities;

namespace StudentPortal.Web.Models
{
    public class Subject
    {
        public int SubjectId { get; set; }
        public string SubjectName { get; set; }
        public string Code { get; set; }
        public int Units { get; set; }

        // Relationship to Course (Curriculum is usually grouped by course)
        public int CourseId { get; set; }
        public Course Course { get; set; }
    }
}
