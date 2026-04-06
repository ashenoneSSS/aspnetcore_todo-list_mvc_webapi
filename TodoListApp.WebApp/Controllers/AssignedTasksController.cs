using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoListApp.WebApp.Models;
using TodoListApp.WebApp.Services;

namespace TodoListApp.WebApp.Controllers;

/// <summary>
/// Controller for viewing and managing tasks assigned to the current user.
/// </summary>
[Authorize]
public class AssignedTasksController : Controller
{
    private readonly ITodoItemWebApiService _todoItemService;
    private readonly ILogger<AssignedTasksController> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="AssignedTasksController"/> class.
    /// </summary>
    /// <param name="todoItemService">The todo item Web API service.</param>
    /// <param name="logger">The logger.</param>
    public AssignedTasksController(ITodoItemWebApiService todoItemService, ILogger<AssignedTasksController> logger)
    {
        _todoItemService = todoItemService;
        _logger = logger;
    }

    /// <summary>
    /// Displays tasks assigned to the current user.
    /// </summary>
    /// <param name="statusFilter">Optional filter: "0", "1", "2" for status, or null for all active (Status != 2).</param>
    /// <param name="sortBy">Sort by "title" or "duedate".</param>
    public async Task<IActionResult> Index(string? statusFilter, string? sortBy)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
        var items = (await _todoItemService.GetAssignedToUserAsync(userId)).ToList();

        if (string.IsNullOrEmpty(statusFilter))
        {
            items = items.Where(i => i.Status != 2).ToList();
        }
        else if (int.TryParse(statusFilter, out int status))
        {
            items = items.Where(i => i.Status == status).ToList();
        }

        items = sortBy?.ToLowerInvariant() switch
        {
            "title" => items.OrderBy(i => i.Title).ToList(),
            "duedate" => items.OrderBy(i => i.DueDate ?? DateTime.MaxValue).ToList(),
            _ => items.OrderBy(i => i.DueDate ?? DateTime.MaxValue).ThenBy(i => i.Title).ToList(),
        };

        ViewData["StatusFilter"] = statusFilter ?? "active";
        ViewData["SortBy"] = sortBy ?? "duedate";
        return View(items);
    }

    /// <summary>
    /// Changes the status of a task.
    /// </summary>
    /// <param name="taskId">The task identifier.</param>
    /// <param name="newStatus">The new status (0, 1, 2).</param>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangeStatus(int taskId, int newStatus)
    {
        var item = await _todoItemService.GetByIdAsync(taskId);
        if (item == null)
        {
            return NotFound();
        }

        item.Status = newStatus;
        await _todoItemService.UpdateAsync(item);
        return RedirectToAction(nameof(Index));
    }
}
