using System.ComponentModel.DataAnnotations;

namespace AIFitnessTutor.Models
{
    public class ActivityPlan
    {
        [Key]
        public int Id { get; set; }
        public string ActivityGoalCategory { get; set; }
        public string? ActivityTimeCategory { get; set; }
        public string[]? ActivityDays { get; set; }
    }
}
