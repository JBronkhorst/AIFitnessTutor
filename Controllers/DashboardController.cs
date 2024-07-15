using AIFitnessTutor.Data;
using AIFitnessTutor.Models;
using AIFitnessTutor.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AIFitnessTutor.Controllers
{
    public class DashboardController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ApplicationDbContext _context;

        public DashboardController(UserManager<AppUser> userManager, ApplicationDbContext context) 
        {
            _userManager = userManager;
            _context = context;
        }

        private async Task<FitnessPlanViewModel> GetFitnessPlanViewModel()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return null;
            }

            var userWithDetails = await _context.Users
                .Include(u => u.PersonalInfo)
                .Include(u => u.Exercises)
                .Include(u => u.Recipes)
                .FirstOrDefaultAsync(u => u.Id == user.Id);

            if (userWithDetails == null)
            {
                return null;
            }

            var exercisesPerDay = userWithDetails.Exercises
                .GroupBy(e => e.DayOfWeek)
                .ToDictionary(g => g.Key, g => g.ToList());

            var mealsPerDay = userWithDetails.Recipes
                .GroupBy(r => r.DayOfWeek)
                .ToDictionary(g => g.Key, g => g.ToList());

            return new FitnessPlanViewModel
            {
                Exercises = exercisesPerDay,
                Meals = mealsPerDay
            };
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var viewModel = await GetFitnessPlanViewModel();
            if (viewModel == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Retrieve BMI data for the specific user
            var bmiData = await GetBMIData(user);

            // Add BMI data to the view model
            viewModel.BMIDates = bmiData.Item1;
            viewModel.BMIScores = bmiData.Item2;
            viewModel.WeightValues = bmiData.Item3;
            viewModel.WaistValues = bmiData.Item4;

            return View(viewModel);
        }

        private async Task<(string[], float[], float[], float[])> GetBMIData(AppUser user)
        {
            var userWithDetails = await _context.Users
                .Include(u => u.BMIs)
                .FirstOrDefaultAsync(u => u.Id == user.Id);

            if (userWithDetails == null || userWithDetails.BMIs == null)
            {
                return (Array.Empty<string>(), Array.Empty<float>(), Array.Empty<float>(), Array.Empty<float>());
            }

            var bmiItems = userWithDetails.BMIs.OrderBy(b => b.MeasureDate).ToList();

            // Extract data for the graph
            var dates = bmiItems.Select(b => b.MeasureDate.ToShortDateString()).ToArray();
            var bmiScores = bmiItems.Select(b => b.BMIScore ?? 0).ToArray();
            var weightValues = bmiItems.Select(b => b.Weight ?? 0).ToArray();
            var waistValues = bmiItems.Select(b => b.WaistCircumference).ToArray();

            return (dates, bmiScores, weightValues, waistValues);
        }

        [HttpGet]
        public async Task<IActionResult> FitnessPlanOverview()
        {
            var viewModel = await GetFitnessPlanViewModel();
            if (viewModel == null)
            {
                return RedirectToAction("Login", "Account");
            }

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult EditFitnessPlan()
        {
            return View();
        }
    }
}
