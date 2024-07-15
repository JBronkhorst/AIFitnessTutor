using Microsoft.AspNetCore.Identity;

namespace AIFitnessTutor.Models
{
    public class AppUser : IdentityUser
    {
        public PersonalInfo PersonalInfo { get; set; }
        public ActivityPlan ActivityPlan { get; set; }
        public NutritionPlan? NutritionPlan { get; set; }
        public ICollection<Exercise> Exercises { get; set; } = new List<Exercise>();
        public ICollection<Recipe> Recipes { get; set; } = new List<Recipe>();
        public ICollection<BMI>? BMIs { get; set; } = new List<BMI>();
    }

}
