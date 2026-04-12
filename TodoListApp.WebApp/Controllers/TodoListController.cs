using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoListApp.WebApp.Models;
using TodoListApp.WebApp.Services;

namespace TodoListApp.WebApp.Controllers;

[Authorize]
public class TodoListController : Controller
{
    private readonly ITodoListWebApiService todoListService;

    public TodoListController(ITodoListWebApiService todoListService)
    {
        this.todoListService = todoListService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
        var lists = await this.todoListService.GetAllAsync(userId);
        var result = lists.ToList();

        // Virtual list for tasks assigned to the current user.
        result.Insert(0, new TodoListWebApiModel
        {
            Id = -1,
            Title = "Assigned to me",
            Description = "Tasks assigned to you (virtual list).",
            UserId = userId,
        });

        return this.View(result);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return this.View(new TodoListWebApiModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public Task<IActionResult> Create(TodoListWebApiModel model)
    {
        ArgumentNullException.ThrowIfNull(model);
        return this.CreateCoreAsync(model);
    }

    [HttpGet]
    public Task<IActionResult> Edit(int id) => this.ShowListViewAsync(id, "Edit");

    [HttpPost]
    [ValidateAntiForgeryToken]
    public Task<IActionResult> Edit(int id, TodoListWebApiModel model)
    {
        ArgumentNullException.ThrowIfNull(model);
        return this.EditCoreAsync(id, model);
    }

    [HttpGet]
    public Task<IActionResult> Delete(int id) => this.ShowListViewAsync(id, "Delete");

    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await this.todoListService.DeleteAsync(id);
        return this.RedirectToAction(nameof(this.Index));
    }

    private async Task<IActionResult> CreateCoreAsync(TodoListWebApiModel model)
    {
        if (this.ModelState.IsValid)
        {
            model.UserId = this.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
            await this.todoListService.CreateAsync(model);
            return this.RedirectToAction(nameof(this.Index));
        }

        return this.View(model);
    }

    private async Task<IActionResult> EditCoreAsync(int id, TodoListWebApiModel model)
    {
        if (id != model.Id)
        {
            return this.BadRequest();
        }

        if (this.ModelState.IsValid)
        {
            await this.todoListService.UpdateAsync(model);
            return this.RedirectToAction(nameof(this.Index));
        }

        return this.View(model);
    }

    private async Task<IActionResult> ShowListViewAsync(int id, string viewName)
    {
        var list = await this.todoListService.GetByIdAsync(id);
        if (list == null)
        {
            return this.NotFound();
        }

        return this.View(viewName, list);
    }
}
