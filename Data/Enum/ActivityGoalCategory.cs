using System.ComponentModel.DataAnnotations;

namespace AIFitnessTutor.Data.Enum
{
    public enum ActivityGoalCategory
    {
        [Display(Name = "Lose weight")]
        LoseWeight,
        [Display(Name = "Keep shape")]
        KeepShape,
        [Display(Name = "Gain more muscle")]
        GainMoreMuscle
    }
}
