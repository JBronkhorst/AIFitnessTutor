using AIFitnessTutor.Models;

namespace AIFitnessTutor.ViewModels
{
    public class FitnessPlanViewModel
    {
        public Dictionary<string, List<Exercise>> Exercises { get; set; }
        public Dictionary<string, List<Recipe>> Meals { get; set; }

        // Add properties for graph data
        public string[] BMIDates { get; set; }
        public float[] BMIScores { get; set; }
        public float[] WeightValues { get; set; }
        public float[] WaistValues { get; set; }

        public FitnessPlanViewModel()
        {
            Exercises = new Dictionary<string, List<Exercise>>();
            Meals = new Dictionary<string, List<Recipe>>();
        }
        public PersonalInfo PersonalInfo { get; set; }
    }

}
