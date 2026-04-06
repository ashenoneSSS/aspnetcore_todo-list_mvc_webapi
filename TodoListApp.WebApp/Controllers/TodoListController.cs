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
    private readonly ITodoListWebApiService _todoListService;
    private readonly ILogger<TodoListController> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="TodoListController"/> class.
    /// </summary>
    /// <param name="todoListService">The todo list Web API service.</param>
    /// <param name="logger">The logger.</param>
    public TodoListController(ITodoListWebApiService todoListService, ILogger<TodoListController> logger)
    {
        _todoListService = todoListService;
        _logger = logger;
    }

    /// <summary>
    /// Displays the list of user's todo lists.
    /// </summary>
    /// <returns>The index view.</returns>
    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
        var lists = await _todoListService.GetAllAsync(userId);
        return View(lists.ToList());
    }

    /// <summary>
    /// Displays the create form.
    /// </summary>
    public IActionResult Create()
    {
        return View(new TodoListWebApiModel());
    }

    /// <summary>
    /// Handles create form submission.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TodoListWebApiModel model)
    {
        if (ModelState.IsValid)
        {
            model.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
            await _todoListService.CreateAsync(model);
            return RedirectToAction(nameof(Index));
        }

        return View(model);
    }

    /// <summary>
    /// Displays the edit form.
    /// </summary>
    public async Task<IActionResult> Edit(int id)
    {
        var list = await _todoListService.GetByIdAsync(id);
        if (list == null)
        {
            return NotFound();
        }

        return View(list);
    }

    /// <summary>
    /// Handles edit form submission.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, TodoListWebApiModel model)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        if (ModelState.IsValid)
        {
            await _todoListService.UpdateAsync(model);
            return RedirectToAction(nameof(Index));
        }

        return View(model);
    }

    /// <summary>
    /// Displays the delete confirmation page.
    /// </summary>
    public async Task<IActionResult> Delete(int id)
    {
        var list = await _todoListService.GetByIdAsync(id);
        if (list == null)
        {
            return NotFound();
        }

        return View(list);
    }

    /// <summary>
    /// Handles delete confirmation.
    /// </summary>
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _todoListService.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }
}
