using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TodoListApp.WebApi.Exceptions;
using TodoListApp.WebApi.Models;
using TodoListApp.WebApi.Services;

namespace TodoListApp.WebApi.Controllers;

/// <summary>
/// REST API controller for todo list operations.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public partial class TodoListController : ControllerBase
{
    private readonly ITodoListDatabaseService service;
    private readonly ILogger<TodoListController> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="TodoListController"/> class.
    /// </summary>
    /// <param name="service">The todo list database service.</param>
    /// <param name="logger">The logger.</param>
    public TodoListController(ITodoListDatabaseService service, ILogger<TodoListController> logger)
    {
        this.service = service;
        this.logger = logger;
    }

    /// <summary>
    /// Gets all todo lists for a user.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <returns>List of todo lists.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<TodoListModel>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<TodoListModel>>> GetAll([FromQuery] string userId)
    {
        var lists = await this.service.GetAllAsync(userId);
        return this.Ok(lists);
    }

    /// <summary>
    /// Gets a todo list by identifier.
    /// </summary>
    /// <param name="id">The todo list identifier.</param>
    /// <returns>The todo list or 404 if not found.</returns>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(TodoListModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TodoListModel>> GetById(int id)
    {
        try
        {
            var list = await this.service.GetByIdAsync(id);
            if (list == null)
            {
                return this.NotFound();
            }

            return this.Ok(list);
        }
        catch (Exception ex)
        {
            LogErrorGettingList(this.logger, id, ex);
            return this.StatusCode(500);
        }
    }

    /// <summary>
    /// Creates a new todo list.
    /// </summary>
    /// <param name="model">The todo list model.</param>
    /// <returns>The created todo list with 201 status.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(TodoListModel), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public Task<ActionResult<TodoListModel>> Create([FromBody] TodoListModel model)
    {
        ArgumentNullException.ThrowIfNull(model);
        return this.CreateCoreAsync(model);
    }

    /// <summary>
    /// Updates an existing todo list.
    /// </summary>
    /// <param name="id">The todo list identifier.</param>
    /// <param name="model">The todo list model.</param>
    /// <returns>204 No Content on success, 404 if not found.</returns>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public Task<IActionResult> Update(int id, [FromBody] TodoListModel model)
    {
        ArgumentNullException.ThrowIfNull(model);
        return this.UpdateCoreAsync(id, model);
    }

    /// <summary>
    /// Deletes a todo list.
    /// </summary>
    /// <param name="id">The todo list identifier.</param>
    /// <returns>204 No Content on success, 404 if not found.</returns>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await this.service.DeleteAsync(id);
            return this.NoContent();
        }
        catch (NotFoundException)
        {
            return this.NotFound();
        }
        catch (Exception ex)
        {
            LogErrorDeletingList(this.logger, id, ex);
            return this.StatusCode(500);
        }
    }

    [LoggerMessage(LogLevel.Error, "Error getting todo list {Id}")]
    private static partial void LogErrorGettingList(ILogger logger, int id, Exception ex);

    [LoggerMessage(LogLevel.Warning, "Not found during create")]
    private static partial void LogWarningNotFoundCreate(ILogger logger, Exception ex);

    [LoggerMessage(LogLevel.Error, "Error creating todo list")]
    private static partial void LogErrorCreatingList(ILogger logger, Exception ex);

    [LoggerMessage(LogLevel.Error, "Error updating todo list {Id}")]
    private static partial void LogErrorUpdatingList(ILogger logger, int id, Exception ex);

    [LoggerMessage(LogLevel.Error, "Error deleting todo list {Id}")]
    private static partial void LogErrorDeletingList(ILogger logger, int id, Exception ex);

    private async Task<ActionResult<TodoListModel>> CreateCoreAsync(TodoListModel model)
    {
        try
        {
            model.CreatedDate = DateTime.UtcNow;
            var created = await this.service.CreateAsync(model);
            return this.CreatedAtAction(nameof(this.GetById), new { id = created.Id }, created);
        }
        catch (NotFoundException ex)
        {
            LogWarningNotFoundCreate(this.logger, ex);
            return this.NotFound();
        }
        catch (Exception ex)
        {
            LogErrorCreatingList(this.logger, ex);
            return this.StatusCode(500);
        }
    }

    private async Task<IActionResult> UpdateCoreAsync(int id, TodoListModel model)
    {
        try
        {
            model.Id = id;
            await this.service.UpdateAsync(model);
            return this.NoContent();
        }
        catch (NotFoundException)
        {
            return this.NotFound();
        }
        catch (Exception ex)
        {
            LogErrorUpdatingList(this.logger, id, ex);
            return this.StatusCode(500);
        }
    }
}
