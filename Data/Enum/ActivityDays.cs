using System.ComponentModel.DataAnnotations;

namespace AIFitnessTutor.Data.Enum
{
    public enum ActivityDays
    {
        [Display(Name = "Sunday")]
        Sunday,
        [Display(Name = "Monday")]
        Monday,
        [Display(Name = "Tuesday")]
        Tuesday,
        [Display(Name = "Wednesday")]
        Wednesday,
        [Display(Name = "Thursday")]
        Thursday,
        [Display(Name = "Friday")]
        Friday,
        [Display(Name = "Saturday")]
        Saturday,
        [Display(Name = "None")]
        None
    }
}
