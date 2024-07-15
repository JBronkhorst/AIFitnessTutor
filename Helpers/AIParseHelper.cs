using AIFitnessTutor.Models;

namespace AIFitnessTutor.Helpers
{
    public static class AIParseHelper
    {
        public static List<Exercise> ParseExercisesFromPlan(string fitnessPlan)
        {
            var exercises = new List<Exercise>();
            var lines = fitnessPlan.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            Exercise currentExercise = null;

            foreach (var line in lines)
            {
                if (line.StartsWith("Day:"))
                {
                    if (currentExercise != null)
                    {
                        exercises.Add(currentExercise);
                    }
                    currentExercise = new Exercise
                    {
                        DayOfWeek = line.Replace("Day:", "").Trim(),
                        StepByStepGuide = string.Empty
                    };
                }
                else if (line.StartsWith("Title:"))
                {
                    if (currentExercise != null)
                    {
                        currentExercise.Name = line.Replace("Title:", "").Trim();
                    }
                }
                else if (line.StartsWith("Step-by-step guide:"))
                {
                    continue;
                }
                else
                {
                    if (currentExercise != null)
                    {
                        currentExercise.StepByStepGuide += line.Trim() + "\n";
                    }
                }
            }

            if (currentExercise != null)
            {
                exercises.Add(currentExercise);
            }

            return exercises;
        }

        public static List<Recipe> ParseRecipesFromPlan(string nutritionPlan)
        {
            var recipes = new List<Recipe>();
            var lines = nutritionPlan.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            Recipe currentRecipe = null;

            foreach (var line in lines)
            {
                if (line.StartsWith("Day:"))
                {
                    if (currentRecipe != null)
                    {
                        recipes.Add(currentRecipe);
                    }
                    currentRecipe = new Recipe
                    {
                        DayOfWeek = line.Replace("Day:", "").Trim(),
                        StepByStepInstructions = string.Empty
                    };
                }
                else if (line.StartsWith("Meal:"))
                {
                    if (currentRecipe != null)
                    {
                        currentRecipe.Type = line.Replace("Meal:", "").Trim();
                    }
                }
                else if (line.StartsWith("Title:"))
                {
                    if (currentRecipe != null)
                    {
                        currentRecipe.Name = line.Replace("Title:", "").Trim();
                    }
                }
                else if (line.StartsWith("Ingredients and Instructions:"))
                {
                    continue;
                }
                else
                {
                    if (currentRecipe != null)
                    {
                        currentRecipe.StepByStepInstructions += line.Trim() + "\n";
                    }
                }
            }

            if (currentRecipe != null)
            {
                recipes.Add(currentRecipe);
            }

            return recipes;
        }
    }
}
