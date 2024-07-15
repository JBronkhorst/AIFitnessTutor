namespace AIFitnessTutor.Helpers
{
    public class AgeCalculatorHelper
    {
        public static string CalculateAge(DateOnly dateOfBirth)
        {
            if (dateOfBirth == DateOnly.MinValue)
            {
                return "Unknown"; // or handle as needed
            }

            DateOnly today = DateOnly.FromDateTime(DateTime.Today);
            int age = today.Year - dateOfBirth.Year;

            // Check if the birthday for this year has not occurred yet
            if (dateOfBirth.DayOfYear > today.DayOfYear)
            {
                age--;
            }

            return age.ToString(); // Convert age to string
        }
    }
}
