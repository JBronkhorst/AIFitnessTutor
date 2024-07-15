using System.Net.Http.Headers;
using System.Text;
using AIFitnessTutor.Helpers;
using AIFitnessTutor.Models;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Identity.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AIFitnessTutor.Services
{
    public class OpenAIService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private const string ChatGPTApiUrl = "https://api.openai.com/v1/chat/completions";
        private const int MaxRetries = 10; // Increase the number of retries
        private const int InitialDelay = 2000; // Increase the initial delay to 2 seconds

        public OpenAIService(HttpClient httpClient, string apiKey)
        {
            if (string.IsNullOrEmpty(apiKey))
            {
                throw new ArgumentException("API key is required.", nameof(apiKey));
            }

            _httpClient = httpClient;
            _apiKey = apiKey;
        }

        public async Task<string> GeneratePlanAsync(string prompt)
        {
            int retries = 0;
            int delay = InitialDelay;

            while (retries < MaxRetries)
            {
                try
                {
                    var request = new
                    {
                        model = "gpt-3.5-turbo",
                        messages = new[]
                        {
                            new { role = "system", content = "You are a helpful assistant." },
                            new { role = "user", content = prompt }
                        },
                        max_tokens = 4000
                    };

                    var jsonRequest = JsonConvert.SerializeObject(request);
                    var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

                    // Log the request JSON
                    Console.WriteLine($"JSON Request: {jsonRequest}");

                    var response = await _httpClient.PostAsync(ChatGPTApiUrl, content);

                    // Log the status code and response content
                    Console.WriteLine($"Response Status Code: {response.StatusCode}");
                    var responseBody = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Response Body: {responseBody}");

                    if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                    {
                        // Log the rate limit issue
                        Console.WriteLine("Rate limit exceeded. Retrying...");
                        await Task.Delay(delay);
                        delay *= 2; // Exponential backoff
                        retries++;
                        continue;
                    }

                    response.EnsureSuccessStatusCode(); // This will throw an exception if the status code is not 2xx

                    var result = await response.Content.ReadAsStringAsync();
                    var json = JObject.Parse(result);

                    // Log the parsed JSON response
                    Console.WriteLine($"Parsed JSON Response: {json}");

                    return json["choices"][0]["message"]["content"].ToString().Trim();
                }
                catch (HttpRequestException ex)
                {
                    // Log the exception details
                    Console.WriteLine($"HttpRequestException: {ex.Message}");
                    if (ex.InnerException != null)
                    {
                        Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                    }
                    throw new Exception("Error occurred while communicating with OpenAI API.", ex);
                }
                catch (JsonReaderException ex)
                {
                    // Log the exception details
                    Console.WriteLine($"JsonReaderException: {ex.Message}");
                    throw new Exception("Error occurred while parsing response from OpenAI API.", ex);
                }
            }

            throw new Exception("Maximum retry attempts exceeded. Please try again later.");
        }

        public async Task<string> GenerateFitnessPlanAsync(string height, string weight, DateOnly dateofbirth, string gender, string activityGoal, string activityTime, string activityDaysJson)
        {
            // Convert date of birth into age
            string age = AgeCalculatorHelper.CalculateAge(dateofbirth);

            // Convert JSON array string to a List of strings
            var activityDaysList = JsonConvert.DeserializeObject<List<string>>(activityDaysJson);

            // Join the activity days list into a clean string separated by commas
            string activityDays = string.Join(", ", activityDaysList);

            string prompt = $"Generate a weekly fitness plan for a {age} year old {gender} with height {height} cm and weight {weight} kg. The fitness goal is {activityGoal}. The user exercises for {activityTime} per day on {activityDays}. Use the following format:\n" +
                    "Day: [Day of the week]\n" +
                    "Title: [Exercise title]\n" +
                    "Step-by-step guide:\n" +
                    "1. [Step 1]\n" +
                    "2. [Step 2]\n";
            return await GeneratePlanAsync(prompt);
        }

        public async Task<string> GenerateNutritionPlanAsync(string nutritionRestrictionsJson, string nutritionAllergiesJson, string activityGoal)
        {
            // Convert JSON array string to a List of strings
            var nutritionRestrictionsList = JsonConvert.DeserializeObject<List<string>>(nutritionRestrictionsJson);
            var nutritionAllergiesList = JsonConvert.DeserializeObject<List<string>>(nutritionAllergiesJson);

            // Join the restrictions and allergies list into a clean string separated by commas
            string nutritionRestrictions = string.Join(", ", nutritionRestrictionsList);
            string nutritionAllergies = string.Join(", ", nutritionAllergiesList);

            var tasks = new List<Task<string>>();

            foreach (var dayOfWeek in Enum.GetValues(typeof(DayOfWeek)))
            {
                var dailyPlanTasks = new List<Task<string>>();

                // Generate breakfast plan
                string breakfastPrompt = $"Generate a nutrition plan for {dayOfWeek} with breakfast for a user with the following dietary restrictions: {nutritionRestrictions} and allergies: {nutritionAllergies}. The fitness goal is {activityGoal}. Use the following format strictly:\n" +
                    "Day: [Day of the week]\n" +
                    "Meal: Breakfast\n" +
                    "Title: [Meal title]\n" +
                    "Ingredients and Instructions:\n" +
                    "1. [Step 1]\n" +
                    "2. [Step 2]\n";
                dailyPlanTasks.Add(GeneratePlanAsync(breakfastPrompt));

                // Generate lunch plan
                string lunchPrompt = $"Generate a nutrition plan for {dayOfWeek} with lunch for a user with the following dietary restrictions: {nutritionRestrictions} and allergies: {nutritionAllergies}. The fitness goal is {activityGoal}. Use the following format strictly:\n" +
                    "Day: [Day of the week]\n" +
                    "Meal: Lunch\n" +
                    "Title: [Meal title]\n" +
                    "Ingredients and Instructions:\n" +
                    "1. [Step 1]\n" +
                    "2. [Step 2]\n";
                dailyPlanTasks.Add(GeneratePlanAsync(lunchPrompt));

                // Generate dinner plan
                string dinnerPrompt = $"Generate a nutrition plan for {dayOfWeek} with dinner for a user with the following dietary restrictions: {nutritionRestrictions} and allergies: {nutritionAllergies}. The fitness goal is {activityGoal}. Use the following format strictly:\n" +
                    "Day: [Day of the week]\n" +
                    "Meal: Dinner\n" +
                    "Title: [Meal title]\n" +
                    "Ingredients and Instructions:\n" +
                    "1. [Step 1]\n" +
                    "2. [Step 2]\n";
                dailyPlanTasks.Add(GeneratePlanAsync(dinnerPrompt));

                // Generate snack plan
                string snackPrompt = $"Generate a nutrition plan for {dayOfWeek} with snack for a user with the following dietary restrictions: {nutritionRestrictions} and allergies: {nutritionAllergies}. The fitness goal is {activityGoal}. Use the following format strictly:\n" +
                    "Day: [Day of the week]\n" +
                    "Meal: Snack\n" +
                    "Title: [Meal title]\n" +
                    "Ingredients and Instructions:\n" +
                    "1. [Step 1]\n" +
                    "2. [Step 2]\n";
                dailyPlanTasks.Add(GeneratePlanAsync(snackPrompt));

                tasks.Add(Task.WhenAll(dailyPlanTasks).ContinueWith(task =>
                {
                    // Concatenate daily meal plans into a single string
                    return string.Join("\n", task.Result);
                }));
            }

            // Wait for all tasks to complete
            await Task.WhenAll(tasks);

            // Concatenate individual daily plans into a single string
            string combinedNutritionPlan = string.Join("\n", tasks.Select(t => t.Result));

            return combinedNutritionPlan;
        }
    }
}
