namespace StudentPortal.Web.Models.Entities
{
    public class Subject
    {
        public int SubjectId { get; set; }
        public string SubjectName { get; set; }
        public int InstructorId { get; set; }
        public string InstructorName { get; set; }
    }
}
