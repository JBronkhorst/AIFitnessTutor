using AIFitnessTutor.Data;
using AIFitnessTutor.Data.Enum;
using AIFitnessTutor.Extensions;
using AIFitnessTutor.Helpers;
using AIFitnessTutor.Models;
using AIFitnessTutor.Services;
using AIFitnessTutor.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace AIFitnessTutor.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly OpenAIService _openAIService;
        private readonly ApplicationDbContext _context;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, OpenAIService openAIService, ApplicationDbContext context)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _openAIService = openAIService;
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            var response = new LoginViewModel();
            return View(response);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            if (!ModelState.IsValid) return View(loginViewModel);

            var user = await _userManager.FindByEmailAsync(loginViewModel.EmailAddress);

            if (user != null)
            {
                //User is found, check password
                var passwordCheck = await _userManager.CheckPasswordAsync(user, loginViewModel.Password);
                if (passwordCheck)
                {
                    //Password correct, sign in
                    var result = await _signInManager.PasswordSignInAsync(user, loginViewModel.Password, false, false);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index", "Dashboard");
                    }
                }
                //Password is incorrect
                TempData["Error"] = "Wrong credentials. Please try again";
                return View(loginViewModel);
            }
            //User not found
            TempData["Error"] = "Wrong credentials. Please try again";
            return View(loginViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult ReviewRegistration()
        {
            var emailAndPassword = HttpContext.Session.GetObject<EmailAndPasswordViewModel>("RegisterEmailAndPassword");
            var personalInfo = HttpContext.Session.GetObject<PersonalInfoViewModel>("RegisterPersonalInfo");
            var activityPlan = HttpContext.Session.GetObject<ActivityPlanViewModel>("RegisterActivityPlan");
            var nutritionPlan = HttpContext.Session.GetObject<NutritionPlanViewModel>("RegisterNutritionPlan");

            var viewModel = new ReviewRegistrationViewModel
            {
                EmailAndPassword = emailAndPassword,
                PersonalInfo = personalInfo,
                ActivityPlan = activityPlan,
                NutritionPlan = nutritionPlan
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> CompleteRegistration()
        {
            var emailAndPassword = HttpContext.Session.GetObject<EmailAndPasswordViewModel>("RegisterEmailAndPassword");
            var personalInfo = HttpContext.Session.GetObject<PersonalInfoViewModel>("RegisterPersonalInfo");
            var activityPlan = HttpContext.Session.GetObject<ActivityPlanViewModel>("RegisterActivityPlan");
            var nutritionPlan = HttpContext.Session.GetObject<NutritionPlanViewModel>("RegisterNutritionPlan");

            var user = new AppUser
            {
                UserName = emailAndPassword.EmailAddress,
                Email = emailAndPassword.EmailAddress,
                PersonalInfo = new PersonalInfo
                {
                    FirstName = personalInfo.FirstName,
                    Surname = personalInfo.Surname,
                    DateOfBirth = personalInfo.DateOfBirth,
                    Gender = Enum.GetName(typeof(Gender), personalInfo.Gender),
                    Height = personalInfo.Height,
                    Weight = personalInfo.Weight
                },
                ActivityPlan = new ActivityPlan
                {
                    ActivityGoalCategory = Enum.GetName(typeof(ActivityGoalCategory), activityPlan.ActivityGoalCategory),
                    ActivityTimeCategory = activityPlan.ActivityTimeCategory.HasValue ? Enum.GetName(typeof(ActivityTimeCategory), activityPlan.ActivityTimeCategory.Value) : null,
                    ActivityDays = activityPlan.ActivityDays.Select(day => Enum.GetName(typeof(ActivityDays), day)).ToArray()
                },
                NutritionPlan = new NutritionPlan
                {
                    NutritionRestriction = nutritionPlan.NutritionRestriction?.Select(restriction => Enum.GetName(typeof(NutritionRestriction), restriction)).ToArray() ?? new[] { "None" },
                    NutritionAllergy = nutritionPlan.NutritionAllergy?.Select(allergy => Enum.GetName(typeof(NutritionAllergy), allergy)).ToArray() ?? new[] { "None" }
                }
            };

            string activityDaysJson = JsonConvert.SerializeObject(activityPlan.ActivityDays.Select(day => Enum.GetName(typeof(ActivityDays), day)).ToList());

            if (!user.ActivityPlan.ActivityDays.Contains("None"))
            {
                var fitnessPlan = await _openAIService.GenerateFitnessPlanAsync(
                    personalInfo.Height.ToString(),
                    personalInfo.Weight.ToString(),
                    personalInfo.DateOfBirth ?? DateOnly.MinValue,
                    Enum.GetName(typeof(Gender), personalInfo.Gender),
                    Enum.GetName(typeof(ActivityGoalCategory), activityPlan.ActivityGoalCategory),
                    activityPlan.ActivityTimeCategory.HasValue ? Enum.GetName(typeof(ActivityTimeCategory), activityPlan.ActivityTimeCategory.Value) : null,
                    activityDaysJson
                );
                user.Exercises = AIParseHelper.ParseExercisesFromPlan(fitnessPlan);
            }

            string nutritionRestrictionsJson = JsonConvert.SerializeObject(nutritionPlan.NutritionRestriction?.Select(restriction => Enum.GetName(typeof(NutritionRestriction), restriction)).ToList() ?? new List<string> { "None" });
            string nutritionAllergiesJson = JsonConvert.SerializeObject(nutritionPlan.NutritionAllergy?.Select(allergy => Enum.GetName(typeof(NutritionAllergy), allergy)).ToList() ?? new List<string> { "None" });

            string nutritionPlanJson = await _openAIService.GenerateNutritionPlanAsync(
                nutritionRestrictionsJson,
                nutritionAllergiesJson,
                Enum.GetName(typeof(ActivityGoalCategory), activityPlan.ActivityGoalCategory)
            );
            user.Recipes = AIParseHelper.ParseRecipesFromPlan(nutritionPlanJson);

            var result = await _userManager.CreateAsync(user, emailAndPassword.Password);

            if (result.Succeeded)
            {
                // Sign in the user
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Dashboard");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            var viewModel = new ReviewRegistrationViewModel
            {
                EmailAndPassword = emailAndPassword,
                PersonalInfo = personalInfo,
                ActivityPlan = activityPlan,
                NutritionPlan = nutritionPlan
            };

            return View("ReviewRegistration", viewModel);
        }

    }
}
