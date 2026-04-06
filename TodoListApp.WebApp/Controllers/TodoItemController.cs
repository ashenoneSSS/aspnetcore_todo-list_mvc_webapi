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
    private readonly ITodoItemWebApiService _todoItemService;
    private readonly ITodoListWebApiService _todoListService;
    private readonly ILogger<TodoItemController> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="TodoItemController"/> class.
    /// </summary>
    /// <param name="todoItemService">The todo item Web API service.</param>
    /// <param name="todoListService">The todo list Web API service.</param>
    /// <param name="logger">The logger.</param>
    public TodoItemController(
        ITodoItemWebApiService todoItemService,
        ITodoListWebApiService todoListService,
        ILogger<TodoItemController> logger)
    {
        _todoItemService = todoItemService;
        _todoListService = todoListService;
        _logger = logger;
    }

    /// <summary>
    /// Displays tasks in a todo list.
    /// </summary>
    /// <param name="listId">The todo list identifier.</param>
    public async Task<IActionResult> Index(int listId)
    {
        var list = await _todoListService.GetByIdAsync(listId);
        if (list == null)
        {
            return NotFound();
        }

        ViewData["TodoList"] = list;
        var items = await _todoItemService.GetByListIdAsync(listId, pageSize: 100);
        return View(items.ToList());
    }

    /// <summary>
    /// Displays the create form.
    /// </summary>
    public async Task<IActionResult> Create(int listId)
    {
        var list = await _todoListService.GetByIdAsync(listId);
        if (list == null)
        {
            return NotFound();
        }

        ViewData["TodoList"] = list;
        var model = new TodoItemWebApiModel
        {
            TodoListId = listId,
            Status = 0,
            AssigneeId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty,
        };
        return View(model);
    }

    /// <summary>
    /// Handles create form submission.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TodoItemWebApiModel model)
    {
        if (ModelState.IsValid)
        {
            model.CreatedDate = DateTime.UtcNow;
            if (string.IsNullOrEmpty(model.AssigneeId))
            {
                model.AssigneeId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
            }

            await _todoItemService.CreateAsync(model);
            return RedirectToAction(nameof(Index), new { listId = model.TodoListId });
        }

        var list = await _todoListService.GetByIdAsync(model.TodoListId);
        ViewData["TodoList"] = list;
        return View(model);
    }

    /// <summary>
    /// Displays the edit form.
    /// </summary>
    public async Task<IActionResult> Edit(int id)
    {
        var item = await _todoItemService.GetByIdAsync(id);
        if (item == null)
        {
            return NotFound();
        }

        var list = await _todoListService.GetByIdAsync(item.TodoListId);
        ViewData["TodoList"] = list;
        return View(item);
    }

    /// <summary>
    /// Handles edit form submission.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, TodoItemWebApiModel model)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        if (ModelState.IsValid)
        {
            await _todoItemService.UpdateAsync(model);
            return RedirectToAction(nameof(Index), new { listId = model.TodoListId });
        }

        var list = await _todoListService.GetByIdAsync(model.TodoListId);
        ViewData["TodoList"] = list;
        return View(model);
    }

    /// <summary>
    /// Displays task details.
    /// </summary>
    public async Task<IActionResult> Details(int id)
    {
        var item = await _todoItemService.GetByIdAsync(id);
        if (item == null)
        {
            return NotFound();
        }

        var list = await _todoListService.GetByIdAsync(item.TodoListId);
        ViewData["TodoList"] = list;
        return View(item);
    }

    /// <summary>
    /// Displays the delete confirmation page.
    /// </summary>
    public async Task<IActionResult> Delete(int id)
    {
        var item = await _todoItemService.GetByIdAsync(id);
        if (item == null)
        {
            return NotFound();
        }

        var list = await _todoListService.GetByIdAsync(item.TodoListId);
        ViewData["TodoList"] = list;
        return View(item);
    }

    /// <summary>
    /// Handles delete confirmation.
    /// </summary>
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, int listId)
    {
        await _todoItemService.DeleteAsync(id);
        return RedirectToAction(nameof(Index), new { listId });
    }
}
