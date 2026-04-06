using Microsoft.AspNetCore.Identity;

namespace TodoListApp.WebApp.Models;

/// <summary>
/// Application user extending IdentityUser.
/// </summary>
public class ApplicationUser : IdentityUser
{
    /// <summary>
    /// Gets or sets the display name.
    /// </summary>
    public string? DisplayName { get; set; }
}
