using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TodoListApp.WebApp.Models;

namespace TodoListApp.WebApp.Controllers;

/// <summary>
/// Controller for user account operations (register, login, logout).
/// </summary>
public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ILogger<AccountController> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="AccountController"/> class.
    /// </summary>
    /// <param name="userManager">The user manager.</param>
    /// <param name="signInManager">The sign-in manager.</param>
    /// <param name="logger">The logger.</param>
    public AccountController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ILogger<AccountController> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _logger = logger;
    }

    /// <summary>
    /// Displays the registration form.
    /// </summary>
    /// <returns>The register view.</returns>
    [HttpGet]
    public IActionResult Register()
    {
        return View(new RegisterViewModel());
    }

    /// <summary>
    /// Handles registration form submission.
    /// </summary>
    /// <param name="model">The registration view model.</param>
    /// <returns>Redirect to TodoList/Index on success, or view with errors.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "TodoList");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        return View(model);
    }

    /// <summary>
    /// Displays the login form.
    /// </summary>
    /// <returns>The login view.</returns>
    [HttpGet]
    public IActionResult Login()
    {
        return View(new LoginViewModel());
    }

    /// <summary>
    /// Handles login form submission.
    /// </summary>
    /// <param name="model">The login view model.</param>
    /// <returns>Redirect to TodoList/Index on success, or view with errors.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            var result = await _signInManager.PasswordSignInAsync(
                model.Email,
                model.Password,
                model.RememberMe,
                lockoutOnFailure: false);

            if (result.Succeeded)
            {
                return RedirectToAction("Index", "TodoList");
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
        }

        return View(model);
    }

    /// <summary>
    /// Signs out the current user.
    /// </summary>
    /// <returns>Redirect to Login.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Login");
    }
}
