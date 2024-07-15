using AIFitnessTutor.Data.Enum;
using System.ComponentModel.DataAnnotations;

namespace AIFitnessTutor.ViewModels
{
    public class NutritionPlanViewModel
    {
        [Display(Name = "Nutrition restrictions")]
        public ICollection<NutritionRestriction>? NutritionRestriction { get; set; }
        [Display(Name = "Nutrition allergies")]
        public ICollection<NutritionAllergy>? NutritionAllergy { get; set; }
    }
}
