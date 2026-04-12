using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoListApp.WebApp.Models;
using TodoListApp.WebApp.Services;

namespace TodoListApp.WebApp.Controllers;

[Authorize]
public class SearchController : Controller
{
    private readonly ITodoItemWebApiService todoItemService;

    public SearchController(ITodoItemWebApiService todoItemService)
    {
        this.todoItemService = todoItemService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(
        string? title,
        DateTime? createdDateFrom,
        DateTime? createdDateTo,
        DateTime? dueDateFrom,
        DateTime? dueDateTo)
    {
        var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;

        var hasCriteria = !string.IsNullOrWhiteSpace(title) ||
            createdDateFrom.HasValue ||
            createdDateTo.HasValue ||
            dueDateFrom.HasValue ||
            dueDateTo.HasValue;

        if (!hasCriteria)
        {
            this.ViewData["Results"] = Array.Empty<TodoItemWebApiModel>();
            this.ViewData["SearchTitle"] = title ?? string.Empty;
            this.ViewData["CreatedDateFrom"] = createdDateFrom?.ToString("yyyy-MM-dd") ?? string.Empty;
            this.ViewData["CreatedDateTo"] = createdDateTo?.ToString("yyyy-MM-dd") ?? string.Empty;
            this.ViewData["DueDateFrom"] = dueDateFrom?.ToString("yyyy-MM-dd") ?? string.Empty;
            this.ViewData["DueDateTo"] = dueDateTo?.ToString("yyyy-MM-dd") ?? string.Empty;
            return this.View();
        }

        var items = (await this.todoItemService.SearchAsync(
            userId,
            title,
            createdDateFrom,
            createdDateTo,
            dueDateFrom,
            dueDateTo,
            page: 1,
            pageSize: 50)).ToList();

        this.ViewData["Results"] = items;
        this.ViewData["SearchTitle"] = title ?? string.Empty;
        this.ViewData["CreatedDateFrom"] = createdDateFrom?.ToString("yyyy-MM-dd") ?? string.Empty;
        this.ViewData["CreatedDateTo"] = createdDateTo?.ToString("yyyy-MM-dd") ?? string.Empty;
        this.ViewData["DueDateFrom"] = dueDateFrom?.ToString("yyyy-MM-dd") ?? string.Empty;
        this.ViewData["DueDateTo"] = dueDateTo?.ToString("yyyy-MM-dd") ?? string.Empty;
        return this.View();
    }
}
