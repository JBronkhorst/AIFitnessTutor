using AIFitnessTutor.Data.Enum;
using AIFitnessTutor.Extensions;
using AIFitnessTutor.Models;
using AIFitnessTutor.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AIFitnessTutor.Controllers
{
    public class PersonalInfoController : Controller
    {
        private readonly UserManager<AppUser> _userManager;

        public PersonalInfoController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult RegisterPersonalInfo()
        {
            var emailAndPasswordData = HttpContext.Session.GetObject<EmailAndPasswordViewModel>("RegisterEmailAndPassword");

            if (emailAndPasswordData == null)
                return RedirectToAction("RegisterEmailAndPassword", "EmailAndPassword");

            var viewModel = HttpContext.Session.GetObject<PersonalInfoViewModel>("RegisterPersonalInfo");
            return View(viewModel ?? new PersonalInfoViewModel());
        }

        [HttpPost]
        public IActionResult RegisterPersonalInfo(PersonalInfoViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            HttpContext.Session.SetObject("RegisterPersonalInfo", viewModel);

            return RedirectToAction("RegisterActivityPlan", "ActivityPlan");
        }

        [HttpGet]
        public async Task<IActionResult> EditPersonalInfo()
        {
            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var viewModel = new PersonalInfoViewModel
            {
                FirstName = user.PersonalInfo.FirstName,
                Surname = user.PersonalInfo.Surname,
                DateOfBirth = user.PersonalInfo.DateOfBirth,
                Gender = Enum.Parse<Gender>(user.PersonalInfo.Gender),
                Height = user.PersonalInfo.Height,
                Weight = user.PersonalInfo.Weight
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> EditPersonalInfo(PersonalInfoViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            user.PersonalInfo.FirstName = viewModel.FirstName;
            user.PersonalInfo.Surname = viewModel.Surname;
            user.PersonalInfo.DateOfBirth = viewModel.DateOfBirth;
            user.PersonalInfo.Gender = Enum.GetName(typeof(Gender), viewModel.Gender);
            user.PersonalInfo.Height = viewModel.Height;
            user.PersonalInfo.Weight = viewModel.Weight;

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

        private async Task<AppUser> GetCurrentUserAsync()
        {
            var userId = _userManager.GetUserId(User);
            return await _userManager.Users.Include(u => u.PersonalInfo).FirstOrDefaultAsync(u => u.Id == userId);
        }
    }

}
