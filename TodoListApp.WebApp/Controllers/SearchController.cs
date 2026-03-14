using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TodoListApp.WebApp.Controllers;

/// <summary>
/// Controller for task search (placeholder for EP04).
/// </summary>
[Authorize]
public class SearchController : Controller
{
    /// <summary>
    /// Displays the search page.
    /// </summary>
    /// <returns>The search view.</returns>
    [HttpGet]
    public IActionResult Index()
    {
        return this.View();
    }
}
