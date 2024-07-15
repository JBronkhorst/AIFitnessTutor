using System.ComponentModel.DataAnnotations;

namespace AIFitnessTutor.Data.Enum
{
    public enum Gender
    {
        [Display(Name = "Male")]
        Male,
        [Display(Name = "Female")]
        Female,
        [Display(Name = "Undefined")]
        Undefined,
        [Display(Name = "Prefer not to say")]
        PreferNotToSay
    }
}
