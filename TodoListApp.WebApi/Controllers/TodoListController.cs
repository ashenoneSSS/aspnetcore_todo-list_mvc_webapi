using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
public class TodoListController : ControllerBase
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
            this.logger.LogError(ex, "Error getting todo list {Id}", id);
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
    public async Task<ActionResult<TodoListModel>> Create([FromBody] TodoListModel model)
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
            this.logger.LogError(ex, "Error creating todo list");
            return this.StatusCode(500);
        }
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
    public async Task<IActionResult> Update(int id, [FromBody] TodoListModel model)
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
            this.logger.LogError(ex, "Error updating todo list {Id}", id);
            return this.StatusCode(500);
        }
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
            this.logger.LogError(ex, "Error deleting todo list {Id}", id);
            return this.StatusCode(500);
        }
    }
}
