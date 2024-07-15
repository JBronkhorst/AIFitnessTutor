using AIFitnessTutor.Extensions;
using AIFitnessTutor.Models;
using AIFitnessTutor.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AIFitnessTutor.Controllers
{
    public class EmailAndPasswordController : Controller
    {
        private readonly UserManager<AppUser> _userManager;

        public EmailAndPasswordController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult RegisterEmailAndPassword()
        {
            var viewModel = HttpContext.Session.GetObject<EmailAndPasswordViewModel>("RegisterEmailAndPassword");
            return View(viewModel ?? new EmailAndPasswordViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> RegisterEmailAndPassword(EmailAndPasswordViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var user = await _userManager.FindByEmailAsync(viewModel.EmailAddress);
            if (user != null)
            {
                TempData["Error"] = "This email address is already in use";
                return View(viewModel);
            }

            HttpContext.Session.SetObject("RegisterEmailAndPassword", viewModel);

            return RedirectToAction("RegisterPersonalInfo", "PersonalInfo");
        }
    }

}
