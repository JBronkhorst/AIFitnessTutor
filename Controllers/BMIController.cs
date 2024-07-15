using AIFitnessTutor.Data;
using AIFitnessTutor.Models;
using AIFitnessTutor.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AIFitnessTutor.Controllers
{
    public class BMIController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ApplicationDbContext _context;

        public BMIController(UserManager<AppUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        private float? CalculateBMI(float? height, float? weight)
        {
            if (height.HasValue && weight.HasValue && height.Value != 0)
            {
                // BMI formula: weight (kg) / (height (m) * height (m))
                return weight.Value / ((height.Value / 100) * (height.Value / 100));
            }
            return null;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var userWithDetails = await _context.Users
                .Include(u => u.BMIs)
                .Include(u => u.PersonalInfo)
                .FirstOrDefaultAsync(u => u.Id == user.Id);

            if (userWithDetails == null)
            {
                return NotFound();
            }

            var bmiItems = userWithDetails.BMIs.OrderByDescending(b => b.MeasureDate).ToList();

            var viewModel = new BMIViewModel
            {
                MeasureDate = DateOnly.FromDateTime(DateTime.Now),
                Height = userWithDetails.PersonalInfo?.Height ?? 0,
                Weight = userWithDetails.PersonalInfo?.Weight ?? 0,
                WaistCircumference = 0,
                BMIItems = bmiItems // Ensure this is not null
            };

            // Prepare data for Chart.js in ascending order
            ViewBag.BMIDates = bmiItems.OrderBy(b => b.MeasureDate).Select(b => b.MeasureDate.ToShortDateString()).ToArray();
            ViewBag.BMIScores = bmiItems.OrderBy(b => b.MeasureDate).Select(b => b.BMIScore ?? 0).ToArray();
            ViewBag.WeightValues = bmiItems.OrderBy(b => b.MeasureDate).Select(b => b.Weight ?? 0).ToArray();
            ViewBag.WaistValues = bmiItems.OrderBy(b => b.MeasureDate).Select(b => b.WaistCircumference).ToArray();

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddBMI(BMIViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var userWithDetails = await _context.Users
                .Include(u => u.BMIs)
                .Include(u => u.PersonalInfo)
                .FirstOrDefaultAsync(u => u.Id == user.Id);

            if (userWithDetails == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                // Calculate BMI score
                float? bmiScore = CalculateBMI(model.Height, model.Weight);

                var newBMI = new BMI
                {
                    MeasureDate = model.MeasureDate,
                    Height = model.Height,
                    Weight = model.Weight,
                    WaistCircumference = model.WaistCircumference,
                    BMIScore = bmiScore.HasValue ? (int)bmiScore.Value : 0
                };

                userWithDetails.BMIs.Add(newBMI);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            ViewBag.BMIItems = userWithDetails.BMIs.OrderByDescending(b => b.MeasureDate).ToList();

            return View("Index", model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteBMI(int id)
        {
            var bmiItem = await _context.BMIs.FindAsync(id);
            if (bmiItem == null)
            {
                return NotFound();
            }

            _context.BMIs.Remove(bmiItem);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}
