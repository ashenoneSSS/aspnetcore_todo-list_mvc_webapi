using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoListApp.WebApp.Models;
using TodoListApp.WebApp.Services;

namespace TodoListApp.WebApp.Controllers;

/// <summary>
/// MVC controller for todo item operations.
/// </summary>
[Authorize]
public class TodoItemController : Controller
{
    private readonly ITodoItemWebApiService todoItemService;
    private readonly ITodoListWebApiService todoListService;

    /// <summary>
    /// Initializes a new instance of the <see cref="TodoItemController"/> class.
    /// </summary>
    /// <param name="todoItemService">The todo item Web API service.</param>
    /// <param name="todoListService">The todo list Web API service.</param>
    public TodoItemController(
        ITodoItemWebApiService todoItemService,
        ITodoListWebApiService todoListService)
    {
        this.todoItemService = todoItemService;
        this.todoListService = todoListService;
    }

    /// <summary>
    /// Displays tasks in a todo list.
    /// </summary>
    /// <param name="listId">The todo list identifier.</param>
    [HttpGet]
    public async Task<IActionResult> Index(int listId)
    {
        var list = await this.todoListService.GetByIdAsync(listId);
        if (list == null)
        {
            return this.NotFound();
        }

        this.ViewData["TodoList"] = list;
        var items = await this.todoItemService.GetByListIdAsync(listId, pageSize: 100);
        return this.View(items.ToList());
    }

    /// <summary>
    /// Displays the create form.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Create(int listId)
    {
        var list = await this.todoListService.GetByIdAsync(listId);
        if (list == null)
        {
            return this.NotFound();
        }

        this.ViewData["TodoList"] = list;
        var model = new TodoItemWebApiModel
        {
            TodoListId = listId,
            Status = -1,
            AssigneeId = this.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty,
        };
        return this.View(model);
    }

    /// <summary>
    /// Handles create form submission.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public Task<IActionResult> Create(TodoItemWebApiModel model)
    {
        ArgumentNullException.ThrowIfNull(model);
        return this.CreateCoreAsync(model);
    }

    /// <summary>
    /// Displays the edit form.
    /// </summary>
    [HttpGet]
    public Task<IActionResult> Edit(int id) => this.ShowItemViewAsync(id, "Edit");

    /// <summary>
    /// Handles edit form submission.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public Task<IActionResult> Edit(int id, TodoItemWebApiModel model)
    {
        ArgumentNullException.ThrowIfNull(model);
        return this.EditCoreAsync(id, model);
    }

    /// <summary>
    /// Displays task details.
    /// </summary>
    [HttpGet]
    public Task<IActionResult> Details(int id) => this.ShowItemViewAsync(id, "Details");

    /// <summary>
    /// Displays the delete confirmation page.
    /// </summary>
    [HttpGet]
    public Task<IActionResult> Delete(int id) => this.ShowItemViewAsync(id, "Delete");

    /// <summary>
    /// Handles delete confirmation.
    /// </summary>
    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, int listId)
    {
        await this.todoItemService.DeleteAsync(id);
        return this.RedirectToAction(nameof(this.Index), new { listId });
    }

    private async Task<IActionResult> CreateCoreAsync(TodoItemWebApiModel model)
    {
        if (this.ModelState.IsValid)
        {
            model.CreatedDate = DateTime.UtcNow;
            if (string.IsNullOrEmpty(model.AssigneeId))
            {
                model.AssigneeId = this.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
            }

            await this.todoItemService.CreateAsync(model);
            return this.RedirectToAction(nameof(this.Index), new { listId = model.TodoListId });
        }

        var list = await this.todoListService.GetByIdAsync(model.TodoListId);
        this.ViewData["TodoList"] = list;
        return this.View(model);
    }

    private async Task<IActionResult> EditCoreAsync(int id, TodoItemWebApiModel model)
    {
        if (id != model.Id)
        {
            return this.BadRequest();
        }

        if (this.ModelState.IsValid)
        {
            await this.todoItemService.UpdateAsync(model);
            return this.RedirectToAction(nameof(this.Index), new { listId = model.TodoListId });
        }

        var list = await this.todoListService.GetByIdAsync(model.TodoListId);
        this.ViewData["TodoList"] = list;
        return this.View(model);
    }

    private async Task<(TodoItemWebApiModel? Item, TodoListWebApiModel? List)> GetItemWithListAsync(int itemId)
    {
        var item = await this.todoItemService.GetByIdAsync(itemId);
        if (item == null)
        {
            return (null, null);
        }

        var list = await this.todoListService.GetByIdAsync(item.TodoListId);
        return (item, list);
    }

    private async Task<IActionResult> ShowItemViewAsync(int id, string viewName)
    {
        var (item, list) = await this.GetItemWithListAsync(id);
        if (item == null)
        {
            return this.NotFound();
        }

        this.ViewData["TodoList"] = list;
        return this.View(viewName, item);
    }
}
