namespace StudentPortal.Web.Models.Entities
{
    public class Subject
    {
        public int Id { get; set; }
        public string SubjectName { get; set; } = "";
        public string Description { get; set; } = "";
        public int Units { get; set; }
        public int InstructorId { get; set; }   // foreign key
    }
}
