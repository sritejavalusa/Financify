using Financify.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Financify.ViewModels;
using System.Text.RegularExpressions;  // <-- required for Regex
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Financify.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Register()
        {
            ViewData["CurrentAction"] = "Register";
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Uncomment below to enforce UserId to be alphabets only
                /*
                if (!Regex.IsMatch(model.UserId, "^[a-zA-Z]+$"))
                {
                    ModelState.AddModelError("UserId", "User ID should contain only alphabets.");
                    return View(model);
                }
                */

                // Ensure password is provided to avoid passing a possible null to CreateAsync
                if (string.IsNullOrWhiteSpace(model?.Password))
                {
                    ModelState.AddModelError("Password", "Password is required.");
                    return View(model);
                }

                var user = new ApplicationUser { UserName = model.UserId, FullName = model.FullName, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    return RedirectToAction("Login");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }

        public IActionResult Login()
        {
            ViewData["CurrentAction"] = "Login";
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Uncomment below to enforce UserId to be alphabets only during login
                /*
                if (!Regex.IsMatch(model.UserId, "^[a-zA-Z]+$"))
                {
                    ModelState.AddModelError("UserId", "User ID should contain only alphabets.");
                    return View(model);
                }
                */

                var result = await _signInManager.PasswordSignInAsync(model.UserId, model.Password, model.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "Invalid login attempt.");
            }
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }

        // Profile action to display user details
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);  // Get the current logged-in user
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            var model = new ProfileViewModel
            {
                FullName = user?.FullName ?? string.Empty,
                UserId = user?.UserName ?? string.Empty,
                Email = user?.Email ?? string.Empty
            };

            return View(model);  // Pass user data to the Profile view
        }

        // EditProfile action to handle the editing of user details (GET)
        public async Task<IActionResult> EditProfile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            var model = new EditProfileViewModel
            {
                FullName = user?.FullName ?? string.Empty,
                UserId = user?.UserName ?? string.Empty,
                Email = user?.Email ?? string.Empty
            };

            return View(model);  // Pass user data to the EditProfile view
        }

        // EditProfile action to handle the form submission and save the updated data (POST)
        [HttpPost]
        public async Task<IActionResult> EditProfile(EditProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return RedirectToAction("Login");
                }

                user.FullName = model.FullName;
                user.UserName = model.UserId;
                user.Email = model.Email;

                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = "Profile updated successfully!";
                    return RedirectToAction("Profile");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);  // Return to EditProfile if validation fails
        }
    }
}
