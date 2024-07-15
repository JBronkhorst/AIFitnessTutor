using AIFitnessTutor.Data.Enum;
using System.ComponentModel.DataAnnotations;

namespace AIFitnessTutor.ViewModels
{
    public class ActivityPlanViewModel
    {
        [Display(Name = "Activity goal")]
        [Required(ErrorMessage = "Activity goal is required")]
        public ActivityGoalCategory? ActivityGoalCategory { get; set; }
        [Display(Name = "Activity days")]
        [Required(ErrorMessage = "Activity days is required")]
        public ICollection<ActivityDays>? ActivityDays { get; set; }
        [Display(Name = "Activity time")]
        public ActivityTimeCategory? ActivityTimeCategory { get; set; }
    }
}
