using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoListApp.WebApp.Models;

namespace TodoListApp.WebApp.Controllers;

/// <summary>
/// Home controller.
/// </summary>
[Authorize]
public class HomeController : Controller
{
    /// <summary>
    /// Displays the home page.
    /// </summary>
    [HttpGet]
    public IActionResult Index()
    {
        return this.View();
    }

    /// <summary>
    /// Displays the privacy page.
    /// </summary>
    [HttpGet]
    public IActionResult Privacy()
    {
        return this.View();
    }

    /// <summary>
    /// Displays the error page.
    /// </summary>
    [HttpGet]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    [AllowAnonymous]
    public IActionResult Error()
    {
        return this.View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier });
    }
}
