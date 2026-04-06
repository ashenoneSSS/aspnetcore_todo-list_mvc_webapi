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
    private readonly ITodoListDatabaseService _service;
    private readonly ILogger<TodoListController> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="TodoListController"/> class.
    /// </summary>
    /// <param name="service">The todo list database service.</param>
    /// <param name="logger">The logger.</param>
    public TodoListController(ITodoListDatabaseService service, ILogger<TodoListController> logger)
    {
        _service = service;
        _logger = logger;
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
        var lists = await _service.GetAllAsync(userId);
        return Ok(lists);
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
            var list = await _service.GetByIdAsync(id);
            if (list == null)
            {
                return NotFound();
            }

            return Ok(list);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting todo list {Id}", id);
            return StatusCode(500);
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
        try
        {
            model.CreatedDate = DateTime.UtcNow;
            var created = await _service.CreateAsync(model);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex, "Not found during create");
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating todo list");
            return StatusCode(500);
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
        try
        {
            model.Id = id;
            await _service.UpdateAsync(model);
            return NoContent();
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating todo list {Id}", id);
            return StatusCode(500);
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
            await _service.DeleteAsync(id);
            return NoContent();
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting todo list {Id}", id);
            return StatusCode(500);
        }
    }
}
