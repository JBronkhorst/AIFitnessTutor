using System.ComponentModel.DataAnnotations;

namespace AIFitnessTutor.Data.Enum
{
    public enum ActivityTimeCategory
    {
        [Display(Name = "15 minutes")]
        FifteenMinutes,
        [Display(Name = "30 minutes")]
        ThirtyMinutes,
        [Display(Name = "1 hour")]
        OneHour,
        [Display(Name = "2 hours")]
        TwoHours
    }
}
