using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoListApp.WebApi.Exceptions;
using TodoListApp.WebApi.Models;
using TodoListApp.WebApi.Services;

namespace TodoListApp.WebApi.Controllers;

/// <summary>
/// REST API controller for todo item operations.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TodoItemController : ControllerBase
{
    private readonly ITodoItemDatabaseService service;
    private readonly ILogger<TodoItemController> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="TodoItemController"/> class.
    /// </summary>
    /// <param name="service">The todo item database service.</param>
    /// <param name="logger">The logger.</param>
    public TodoItemController(ITodoItemDatabaseService service, ILogger<TodoItemController> logger)
    {
        this.service = service;
        this.logger = logger;
    }

    /// <summary>
    /// Gets todo items by list identifier with pagination.
    /// </summary>
    /// <param name="listId">The todo list identifier.</param>
    /// <param name="page">Page number (1-based).</param>
    /// <param name="pageSize">Page size.</param>
    /// <returns>List of todo items.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<TodoItemModel>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<TodoItemModel>>> GetByListId(
        [FromQuery] int listId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var items = await this.service.GetByListIdAsync(listId, page, pageSize);
        return this.Ok(items);
    }

    /// <summary>
    /// Gets todo items assigned to a user.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <returns>List of assigned todo items.</returns>
    [HttpGet("assigned")]
    [ProducesResponseType(typeof(IEnumerable<TodoItemModel>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<TodoItemModel>>> GetAssigned([FromQuery] string userId)
    {
        var items = await this.service.GetAssignedToUserAsync(userId);
        return this.Ok(items);
    }

    /// <summary>
    /// Gets a todo item by identifier.
    /// </summary>
    /// <param name="id">The todo item identifier.</param>
    /// <returns>The todo item or 404 if not found.</returns>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(TodoItemModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TodoItemModel>> GetById(int id)
    {
        try
        {
            var item = await this.service.GetByIdAsync(id);
            if (item == null)
            {
                return this.NotFound();
            }

            return this.Ok(item);
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Error getting todo item {Id}", id);
            return this.StatusCode(500);
        }
    }

    /// <summary>
    /// Creates a new todo item.
    /// </summary>
    /// <param name="model">The todo item model.</param>
    /// <returns>The created todo item with 201 status.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(TodoItemModel), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<TodoItemModel>> Create([FromBody] TodoItemModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        try
        {
            model.CreatedDate = DateTime.UtcNow;
            var created = await this.service.CreateAsync(model);
            return this.CreatedAtAction(nameof(this.GetById), new { id = created.Id }, created);
        }
        catch (NotFoundException ex)
        {
            this.logger.LogWarning(ex, "Not found during create");
            return this.NotFound();
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Error creating todo item");
            return this.StatusCode(500);
        }
    }

    /// <summary>
    /// Updates an existing todo item.
    /// </summary>
    /// <param name="id">The todo item identifier.</param>
    /// <param name="model">The todo item model.</param>
    /// <returns>204 No Content on success, 404 if not found.</returns>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Update(int id, [FromBody] TodoItemModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

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
            this.logger.LogError(ex, "Error updating todo item {Id}", id);
            return this.StatusCode(500);
        }
    }

    /// <summary>
    /// Deletes a todo item.
    /// </summary>
    /// <param name="id">The todo item identifier.</param>
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
            this.logger.LogError(ex, "Error deleting todo item {Id}", id);
            return this.StatusCode(500);
        }
    }
}
