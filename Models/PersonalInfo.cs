using System.ComponentModel.DataAnnotations;

namespace AIFitnessTutor.Models
{
    public class PersonalInfo
    {
        [Key]
        public int Id { get; set; }
        public string? FirstName { get; set; }
        public string? Surname { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public string Gender { get; set; }
        public float? Height { get; set; }
        public float? Weight { get; set; }
    }

}
