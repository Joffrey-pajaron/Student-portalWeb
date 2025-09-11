namespace StudentPortal.Web.Models.Entities
{
    public class Instructor
    {
        public int Id { get; set; }   // Changed from Guid → int

        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string Email { get; set; } = "";
        public string Specialization { get; set; } = "";
    }
}
