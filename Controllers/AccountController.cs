using IdentityApp.Models;
using IdentityApp.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<IActionResult> Register(string returnUrl = null)
        {
            var viewModel = new RegisterViewModel
            {
                ReturnUrl = returnUrl ?? Url.Content("~/")
            };
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel viewModel, string returnUrl)
        {
            returnUrl ??= Url.Content("~/");
            viewModel.ReturnUrl = returnUrl;
            if (ModelState.IsValid)
            {
                var user = new AppUser { UserName = viewModel.Email, Email = viewModel.Email };
                var result = await _userManager.CreateAsync(user, viewModel.Password);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return LocalRedirect(returnUrl);
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Login(string returnUrl = null)
        {

            var viewModel = new LoginViewModel
            {
                ReturnUrl = returnUrl ?? Url.Content("~/")
            };
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel viewModel, string returnUrl)
        {

            var result = await _signInManager.PasswordSignInAsync(viewModel.Email, viewModel.Password, viewModel.RememberMe, lockoutOnFailure: false);
            var user = await _userManager.FindByEmailAsync(viewModel.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "User not found.");
                return View(viewModel);
            }

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, viewModel.Password);
            if (!isPasswordValid)
            {
                ModelState.AddModelError(string.Empty, "Invalid password.");
                return View(viewModel);
            }

            if (result.Succeeded)
            {
                return LocalRedirect(returnUrl);
            }
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogOff()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
