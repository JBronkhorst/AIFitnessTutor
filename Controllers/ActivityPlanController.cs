using AIFitnessTutor.Data.Enum;
using AIFitnessTutor.Data;
using AIFitnessTutor.Extensions;
using AIFitnessTutor.Helpers;
using AIFitnessTutor.Models;
using AIFitnessTutor.Services;
using AIFitnessTutor.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;

namespace AIFitnessTutor.Controllers
{
    public class ActivityPlanController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly OpenAIService _openAIService;
        private readonly ApplicationDbContext _context;

        public ActivityPlanController(UserManager<AppUser> userManager, OpenAIService openAIService, ApplicationDbContext context)
        {
            _userManager = userManager;
            _openAIService = openAIService;
            _context = context;

        }

        [HttpGet]
        public IActionResult RegisterActivityPlan()
        {
            var emailAndPasswordData = HttpContext.Session.GetObject<EmailAndPasswordViewModel>("RegisterEmailAndPassword");
            var personalInfoData = HttpContext.Session.GetObject<PersonalInfoViewModel>("RegisterPersonalInfo");

            if (emailAndPasswordData == null || personalInfoData == null)
                return RedirectToAction("RegisterPersonalInfo", "PersonalInfo");

            var viewModel = HttpContext.Session.GetObject<ActivityPlanViewModel>("RegisterActivityPlan");
            return View(viewModel ?? new ActivityPlanViewModel());
        }

        [HttpPost]
        public IActionResult RegisterActivityPlan(ActivityPlanViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            HttpContext.Session.SetObject("RegisterActivityPlan", viewModel);

            return RedirectToAction("RegisterNutritionPlan", "NutritionPlan");
        }

        [HttpGet]
        public async Task<IActionResult> EditActivityPlan()
        {
            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var viewModel = new ActivityPlanViewModel
            {
                ActivityGoalCategory = Enum.Parse<ActivityGoalCategory>(user.ActivityPlan.ActivityGoalCategory),
                ActivityTimeCategory = user.ActivityPlan.ActivityTimeCategory != null ? Enum.Parse<ActivityTimeCategory>(user.ActivityPlan.ActivityTimeCategory) : (ActivityTimeCategory?)null,
                ActivityDays = user.ActivityPlan.ActivityDays.Select(Enum.Parse<ActivityDays>).ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> EditActivityPlan(ActivityPlanViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var user = await _userManager.Users
                                        .Include(u => u.ActivityPlan)
                                        .Include(u => u.PersonalInfo)
                                        .Include(u => u.NutritionPlan)
                                        .Include(u => u.Exercises)
                                        .Include(u => u.Recipes)
                                        .FirstOrDefaultAsync(u => u.Id == _userManager.GetUserId(User));

            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var currentActivityGoal = user.ActivityPlan.ActivityGoalCategory;

            user.ActivityPlan.ActivityGoalCategory = Enum.GetName(typeof(ActivityGoalCategory), viewModel.ActivityGoalCategory);
            user.ActivityPlan.ActivityTimeCategory = viewModel.ActivityTimeCategory.HasValue ? Enum.GetName(typeof(ActivityTimeCategory), viewModel.ActivityTimeCategory.Value) : null;
            user.ActivityPlan.ActivityDays = viewModel.ActivityDays.Select(day => Enum.GetName(typeof(ActivityDays), day)).ToArray();

            if (!user.ActivityPlan.ActivityDays.Contains("None"))
            {
                var fitnessPlan = await RegenerateFitnessPlan(user);
                user.Exercises = AIParseHelper.ParseExercisesFromPlan(fitnessPlan);
            }
            else
            {
                user.Exercises = null;
            }

            if (currentActivityGoal != Enum.GetName(typeof(ActivityGoalCategory), viewModel.ActivityGoalCategory))
            {
                var nutritionPlan = await RegenerateNutritionPlan(user);
                user.Recipes = AIParseHelper.ParseRecipesFromPlan(nutritionPlan);
            }

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Dashboard");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(viewModel);
        }

        private async Task<string> RegenerateFitnessPlan(AppUser user)
        {
            _context.RemoveRange(user.Exercises);
            await _context.SaveChangesAsync();

            string activityDaysJson = JsonConvert.SerializeObject(user.ActivityPlan.ActivityDays);

            string fitnessplan = await _openAIService.GenerateFitnessPlanAsync(
                user.PersonalInfo.Height.ToString(),
                user.PersonalInfo.Weight.ToString(),
                user.PersonalInfo.DateOfBirth ?? DateOnly.MinValue,
                user.PersonalInfo.Gender,
                user.ActivityPlan.ActivityGoalCategory,
                user.ActivityPlan.ActivityTimeCategory,
                activityDaysJson
            );

            return fitnessplan;
        }

        private async Task<string> RegenerateNutritionPlan(AppUser user)
        {
            _context.RemoveRange(user.Recipes);
            await _context.SaveChangesAsync();

            string nutritionRestrictionsJson = JsonConvert.SerializeObject(user.NutritionPlan.NutritionRestriction);
            string nutritionAllergiesJson = JsonConvert.SerializeObject(user.NutritionPlan.NutritionAllergy);

            return await _openAIService.GenerateNutritionPlanAsync(
            nutritionRestrictionsJson,
            nutritionAllergiesJson,
                user.ActivityPlan.ActivityGoalCategory
            );
        }

        private async Task<AppUser> GetCurrentUserAsync()
        {
            var userId = _userManager.GetUserId(User);
            return await _userManager.Users.Include(u => u.ActivityPlan).FirstOrDefaultAsync(u => u.Id == userId);
        }
    }

}
