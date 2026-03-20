using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoListApp.WebApp.Models;
using TodoListApp.WebApp.Services;

namespace TodoListApp.WebApp.Controllers;

/// <summary>
/// Controller for viewing tags (Epic 5).
/// </summary>
[Authorize]
public class TagsController : Controller
{
    private readonly ITagWebApiService tagService;

    /// <summary>
    /// Initializes a new instance of the <see cref="TagsController"/> class.
    /// </summary>
    /// <param name="tagService">The tag Web API service.</param>
    public TagsController(ITagWebApiService tagService)
    {
        this.tagService = tagService;
    }

    /// <summary>
    /// Displays all tags.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
        var tags = (await this.tagService.GetAllAsync(userId)).ToList();
        return this.View(tags);
    }

    /// <summary>
    /// Displays tasks with a specific tag.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Tasks(int id)
    {
        var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
        var tags = (await this.tagService.GetAllAsync(userId)).ToList();
        var tag = tags.FirstOrDefault(t => t.Id == id);
        if (tag == null)
        {
            return this.NotFound();
        }

        var items = (await this.tagService.GetTasksByTagAsync(id, userId)).ToList();
        this.ViewData["Tag"] = tag;
        return this.View(items);
    }
}
