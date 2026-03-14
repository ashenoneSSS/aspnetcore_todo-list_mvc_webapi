using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoListApp.WebApp.Models;
using TodoListApp.WebApp.Services;

namespace TodoListApp.WebApp.Controllers;

/// <summary>
/// MVC controller for todo list operations.
/// </summary>
[Authorize]
public class TodoListController : Controller
{
    private readonly ITodoListWebApiService todoListService;

    /// <summary>
    /// Initializes a new instance of the <see cref="TodoListController"/> class.
    /// </summary>
    /// <param name="todoListService">The todo list Web API service.</param>
    public TodoListController(ITodoListWebApiService todoListService)
    {
        this.todoListService = todoListService;
    }

    /// <summary>
    /// Displays the list of user's todo lists.
    /// </summary>
    /// <returns>The index view.</returns>
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
        var lists = await this.todoListService.GetAllAsync(userId);
        return this.View(lists.ToList());
    }

    /// <summary>
    /// Displays the create form.
    /// </summary>
    [HttpGet]
    public IActionResult Create()
    {
        return this.View(new TodoListWebApiModel());
    }

    /// <summary>
    /// Handles create form submission.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TodoListWebApiModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        if (this.ModelState.IsValid)
        {
            model.UserId = this.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
            await this.todoListService.CreateAsync(model);
            return this.RedirectToAction(nameof(this.Index));
        }

        return this.View(model);
    }

    /// <summary>
    /// Displays the edit form.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var list = await this.todoListService.GetByIdAsync(id);
        if (list == null)
        {
            return this.NotFound();
        }

        return this.View(list);
    }

    /// <summary>
    /// Handles edit form submission.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, TodoListWebApiModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

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

    /// <summary>
    /// Displays the delete confirmation page.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var list = await this.todoListService.GetByIdAsync(id);
        if (list == null)
        {
            return this.NotFound();
        }

        return this.View(list);
    }

    /// <summary>
    /// Handles delete confirmation.
    /// </summary>
    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await this.todoListService.DeleteAsync(id);
        return this.RedirectToAction(nameof(this.Index));
    }
}
