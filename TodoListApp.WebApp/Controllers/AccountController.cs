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
    private readonly UserManager<ApplicationUser> userManager;
    private readonly SignInManager<ApplicationUser> signInManager;

    /// <summary>
    /// Initializes a new instance of the <see cref="AccountController"/> class.
    /// </summary>
    /// <param name="userManager">The user manager.</param>
    /// <param name="signInManager">The sign-in manager.</param>
    public AccountController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager)
    {
        this.userManager = userManager;
        this.signInManager = signInManager;
    }

    /// <summary>
    /// Displays the registration form.
    /// </summary>
    /// <returns>The register view.</returns>
    [HttpGet]
    public IActionResult Register()
    {
        return this.View(new RegisterViewModel());
    }

    /// <summary>
    /// Handles registration form submission.
    /// </summary>
    /// <param name="model">The registration view model.</param>
    /// <returns>Redirect to TodoList/Index on success, or view with errors.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public Task<IActionResult> Register(RegisterViewModel model)
    {
        ArgumentNullException.ThrowIfNull(model);
        return this.RegisterCoreAsync(model);
    }

    /// <summary>
    /// Displays the login form.
    /// </summary>
    /// <returns>The login view.</returns>
    [HttpGet]
    public IActionResult Login()
    {
        return this.View(new LoginViewModel());
    }

    /// <summary>
    /// Handles login form submission.
    /// </summary>
    /// <param name="model">The login view model.</param>
    /// <returns>Redirect to TodoList/Index on success, or view with errors.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public Task<IActionResult> Login(LoginViewModel model)
    {
        ArgumentNullException.ThrowIfNull(model);
        return this.LoginCoreAsync(model);
    }

    /// <summary>
    /// Signs out the current user.
    /// </summary>
    /// <returns>Redirect to Login.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await this.signInManager.SignOutAsync();
        return this.RedirectToAction("Login");
    }

    private async Task<IActionResult> RegisterCoreAsync(RegisterViewModel model)
    {
        if (this.ModelState.IsValid)
        {
            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
            };

            var result = await this.userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await this.signInManager.SignInAsync(user, isPersistent: false);
                return this.RedirectToAction("Index", "TodoList");
            }

            foreach (var error in result.Errors)
            {
                this.ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        return this.View(model);
    }

    private async Task<IActionResult> LoginCoreAsync(LoginViewModel model)
    {
        if (this.ModelState.IsValid)
        {
            var result = await this.signInManager.PasswordSignInAsync(
                model.Email,
                model.Password,
                model.RememberMe,
                lockoutOnFailure: false);

            if (result.Succeeded)
            {
                return this.RedirectToAction("Index", "TodoList");
            }

            this.ModelState.AddModelError(string.Empty, "Invalid login attempt.");
        }

        return this.View(model);
    }
}
