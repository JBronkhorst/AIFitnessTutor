using System.ComponentModel.DataAnnotations;

namespace AIFitnessTutor.Models
{
    public class NutritionPlan
    {
        [Key]
        public int Id { get; set; }
        public string[]? NutritionRestriction { get; set; }
        public string[]? NutritionAllergy { get; set; }
    }
}
