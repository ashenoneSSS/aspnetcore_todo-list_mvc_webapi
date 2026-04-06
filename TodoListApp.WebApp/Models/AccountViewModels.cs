using System.ComponentModel.DataAnnotations;

namespace TodoListApp.WebApp.Models;

/// <summary>
/// View model for user registration.
/// </summary>
public class RegisterViewModel
{
    /// <summary>
    /// Gets or sets the email address.
    /// </summary>
    [Required]
    [EmailAddress]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the password.
    /// </summary>
    [Required]
    [StringLength(100, MinimumLength = 6)]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the password confirmation.
    /// </summary>
    [DataType(DataType.Password)]
    [Display(Name = "Confirm password")]
    [Compare("Password", ErrorMessage = "Passwords do not match.")]
    public string ConfirmPassword { get; set; } = string.Empty;
}

/// <summary>
/// View model for user login.
/// </summary>
public class LoginViewModel
{
    /// <summary>
    /// Gets or sets the email address.
    /// </summary>
    [Required]
    [EmailAddress]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the password.
    /// </summary>
    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether to remember the user.
    /// </summary>
    [Display(Name = "Remember me")]
    public bool RememberMe { get; set; }
}
