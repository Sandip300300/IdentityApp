using IdentityApp.Models;
using IdentityApp.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityApp.Data
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
        public async Task<IActionResult> Register(RegisterViewModel viewModel , string returnUrl) 
        {
            returnUrl ??= Url.Content("~/");
            viewModel.ReturnUrl = returnUrl;
            if (ModelState.IsValid)
            {
                var user = new AppUser { UserName = viewModel.UserName, Email = viewModel.Email };
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
    }
}
