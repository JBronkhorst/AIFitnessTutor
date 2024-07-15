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
    public class NutritionPlanController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly OpenAIService _openAIService;
        private readonly ApplicationDbContext _context;

        public NutritionPlanController(UserManager<AppUser> userManager, OpenAIService openAIService, ApplicationDbContext context)
        {
            _userManager = userManager;
            _openAIService = openAIService;
            _context = context;
        }

        [HttpGet]
        public IActionResult RegisterNutritionPlan()
        {
            var emailAndPasswordData = HttpContext.Session.GetObject<EmailAndPasswordViewModel>("RegisterEmailAndPassword");
            var personalInfoData = HttpContext.Session.GetObject<PersonalInfoViewModel>("RegisterPersonalInfo");
            var activityPlanData = HttpContext.Session.GetObject<ActivityPlanViewModel>("RegisterActivityPlan");

            if (emailAndPasswordData == null || personalInfoData == null || activityPlanData == null)
                return RedirectToAction("RegisterActivityPlan", "ActivityPlan");

            var viewModel = HttpContext.Session.GetObject<NutritionPlanViewModel>("RegisterNutritionPlan");
            return View(viewModel ?? new NutritionPlanViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> RegisterNutritionPlan(NutritionPlanViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            HttpContext.Session.SetObject("RegisterNutritionPlan", viewModel);

            return RedirectToAction("ReviewRegistration", "Account");
        }

        [HttpGet]
        public async Task<IActionResult> EditNutritionPlan()
        {
            var user = await _userManager.Users
                .Include(u => u.NutritionPlan)
                .Include(u => u.ActivityPlan)
                .Include(u => u.PersonalInfo)
                .FirstOrDefaultAsync(u => u.Id == _userManager.GetUserId(User));

            if (user == null)
            {
                return RedirectToAction("Login");
            }

            var viewModel = new NutritionPlanViewModel();

            // Convert arrays of strings to collections of enums
            if (user.NutritionPlan.NutritionRestriction != null && user.NutritionPlan.NutritionRestriction.Any())
            {
                var nutritionRestrictions = user.NutritionPlan.NutritionRestriction
                    .Select(r => Enum.TryParse<NutritionRestriction>(r, out var restriction) ? restriction : (NutritionRestriction?)null)
                    .Where(r => r.HasValue)
                    .Select(r => r.Value)
                    .ToList();

                viewModel.NutritionRestriction = nutritionRestrictions;
            }
            else
            {
                viewModel.NutritionRestriction = new List<NutritionRestriction>();
            }

            if (user.NutritionPlan.NutritionAllergy != null && user.NutritionPlan.NutritionAllergy.Any())
            {
                var nutritionAllergies = user.NutritionPlan.NutritionAllergy
                    .Select(a => Enum.TryParse<NutritionAllergy>(a, out var allergy) ? allergy : (NutritionAllergy?)null)
                    .Where(a => a.HasValue)
                    .Select(a => a.Value)
                    .ToList();

                viewModel.NutritionAllergy = nutritionAllergies;
            }
            else
            {
                viewModel.NutritionAllergy = new List<NutritionAllergy>();
            }

            return View(viewModel);
        }

        [HttpPost]
        public async Task<ActionResult> EditNutritionPlan(NutritionPlanViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.Users
                            .Include(u => u.ActivityPlan)
                            .Include(u => u.PersonalInfo)
                            .Include(u => u.NutritionPlan)
                            .Include(u => u.Recipes)
                            .FirstOrDefaultAsync(u => u.Id == _userManager.GetUserId(User));

            if (user == null)
            {
                return RedirectToAction("Login");
            }

            user.NutritionPlan.NutritionRestriction = model.NutritionRestriction?.Select(restriction => Enum.GetName(typeof(NutritionRestriction), restriction)).ToArray() ?? new[] { "None" };
            user.NutritionPlan.NutritionAllergy = model.NutritionAllergy?.Select(restriction => Enum.GetName(typeof(NutritionAllergy), restriction)).ToArray() ?? new[] { "None" };

            // Regenerate nutrition plan 
            var nutritionPlan = await RegenerateNutritionPlan(user);
            user.Recipes = AIParseHelper.ParseRecipesFromPlan(nutritionPlan);

            // Update the user's nutrition plan
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Dashboard");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
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
            return await _userManager.Users.Include(u => u.NutritionPlan).FirstOrDefaultAsync(u => u.Id == userId);
        }
    }

}
