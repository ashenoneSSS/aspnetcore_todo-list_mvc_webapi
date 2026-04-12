using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoListApp.WebApp.Models;
using TodoListApp.WebApp.Services;

namespace TodoListApp.WebApp.Controllers;

[Authorize]
public class TasksController : Controller
{
    private readonly ITodoItemWebApiService todoItemService;

    public TasksController(ITodoItemWebApiService todoItemService)
    {
        this.todoItemService = todoItemService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? statusFilter, string? sortBy)
    {
        var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
        var items = (await this.todoItemService.GetAllForUserAsync(userId)).ToList();

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
