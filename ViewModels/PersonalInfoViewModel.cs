using AIFitnessTutor.Data.Enum;
using System.ComponentModel.DataAnnotations;

namespace AIFitnessTutor.ViewModels
{
    public class PersonalInfoViewModel
    {
        [Display(Name = "First name")]
        [Required(ErrorMessage = "First name is required")]
        public string FirstName { get; set; }
        [Display(Name = "Surname")]
        [Required(ErrorMessage = "Surname is required")]
        public string Surname { get; set; }
        [Display(Name = "Date of birth")]
        [Required(ErrorMessage = "Date of birth is required")]
        public DateOnly? DateOfBirth { get; set; }
        [Display(Name = "Gender")]
        [Required(ErrorMessage = "Gender is required")]
        public Gender Gender { get; set; }
        [Display(Name = "Height")]
        [Required(ErrorMessage = "Height is required")]
        public float? Height { get; set; }
        [Display(Name = "Weight")]
        [Required(ErrorMessage = "Weight is required")]
        public float? Weight { get; set; }
    }
}
