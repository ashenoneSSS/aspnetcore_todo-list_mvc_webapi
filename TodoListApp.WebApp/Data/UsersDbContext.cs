using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TodoListApp.WebApp.Models;

namespace TodoListApp.WebApp.Data;

/// <summary>
/// Database context for Identity users.
/// </summary>
public class UsersDbContext : IdentityDbContext<ApplicationUser>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UsersDbContext"/> class.
    /// </summary>
    /// <param name="options">The options to be used by the context.</param>
    public UsersDbContext(DbContextOptions<UsersDbContext> options)
        : base(options)
    {
    }
}
