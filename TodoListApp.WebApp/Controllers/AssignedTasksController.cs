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
    private readonly ITodoItemWebApiService todoItemService;

    /// <summary>
    /// Initializes a new instance of the <see cref="AssignedTasksController"/> class.
    /// </summary>
    /// <param name="todoItemService">The todo item Web API service.</param>
    public AssignedTasksController(ITodoItemWebApiService todoItemService)
    {
        this.todoItemService = todoItemService;
    }

    /// <summary>
    /// Displays tasks assigned to the current user.
    /// </summary>
    /// <param name="statusFilter">Optional filter: "0", "1", "2" for status, or null for all active (Status != 2).</param>
    /// <param name="sortBy">Sort by "title" or "duedate".</param>
    [HttpGet]
    public async Task<IActionResult> Index(string? statusFilter, string? sortBy)
    {
        var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
        var items = (await this.todoItemService.GetAssignedToUserAsync(userId)).ToList();

        if (!string.IsNullOrWhiteSpace(statusFilter) && int.TryParse(statusFilter, out int status))
        {
            items = items.Where(i => i.Status == status).ToList();
        }

        items = sortBy?.ToUpperInvariant() switch
        {
            "TITLE" => items.OrderBy(i => i.Title).ToList(),
            "DUEDATE" => items.OrderBy(i => i.DueDate ?? DateTime.MaxValue).ToList(),
            _ => items.OrderBy(i => i.DueDate ?? DateTime.MaxValue).ThenBy(i => i.Title).ToList(),
        };

        this.ViewData["StatusFilter"] = statusFilter ?? string.Empty;
        this.ViewData["SortBy"] = sortBy ?? "duedate";
        return this.View(items);
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
        var item = await this.todoItemService.GetByIdAsync(taskId);
        if (item == null)
        {
            return this.NotFound();
        }

        item.Status = newStatus;
        await this.todoItemService.UpdateAsync(item);
        return this.RedirectToAction(nameof(this.Index));
    }
}
