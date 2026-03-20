using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoListApp.WebApi.Exceptions;
using TodoListApp.WebApi.Models;
using TodoListApp.WebApi.Services;

namespace TodoListApp.WebApi.Controllers;

/// <summary>
/// REST API controller for tag operations (Epic 5).
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TagController : ControllerBase
{
    private readonly ITagDatabaseService service;

    /// <summary>
    /// Initializes a new instance of the <see cref="TagController"/> class.
    /// </summary>
    /// <param name="service">The tag database service.</param>
    public TagController(ITagDatabaseService service)
    {
        this.service = service;
    }

    /// <summary>
    /// Gets all tags for the user's lists.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <returns>List of tags.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<TagModel>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<TagModel>>> GetAll([FromQuery] string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            return this.BadRequest("userId is required.");
        }

        var tags = await this.service.GetAllForUserAsync(userId);
        return this.Ok(tags);
    }

    /// <summary>
    /// Gets tasks that have a specific tag.
    /// </summary>
    /// <param name="id">The tag identifier.</param>
    /// <param name="userId">The user identifier.</param>
    /// <param name="page">Page number.</param>
    /// <param name="pageSize">Page size.</param>
    /// <returns>List of tasks.</returns>
    [HttpGet("{id:int}/tasks")]
    [ProducesResponseType(typeof(IEnumerable<TodoItemModel>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<TodoItemModel>>> GetTasks(
        int id,
        [FromQuery] string userId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            return this.BadRequest("userId is required.");
        }

        var items = await this.service.GetTasksByTagAsync(id, userId, page, pageSize);
        return this.Ok(items);
    }

    /// <summary>
    /// Adds a tag to a task.
    /// </summary>
    /// <param name="todoItemId">The todo item identifier.</param>
    /// <param name="tagId">The tag identifier.</param>
    /// <returns>204 No Content on success.</returns>
    [HttpPost("todoitem/{todoItemId:int}/tag/{tagId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddTagToTask(int todoItemId, int tagId)
    {
        try
        {
            await this.service.AddTagToTaskAsync(todoItemId, tagId);
            return this.NoContent();
        }
        catch (NotFoundException)
        {
            return this.NotFound();
        }
    }

    /// <summary>
    /// Removes a tag from a task.
    /// </summary>
    /// <param name="todoItemId">The todo item identifier.</param>
    /// <param name="tagId">The tag identifier.</param>
    /// <returns>204 No Content on success.</returns>
    [HttpDelete("todoitem/{todoItemId:int}/tag/{tagId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveTagFromTask(int todoItemId, int tagId)
    {
        try
        {
            await this.service.RemoveTagFromTaskAsync(todoItemId, tagId);
            return this.NoContent();
        }
        catch (NotFoundException)
        {
            return this.NotFound();
        }
    }

    /// <summary>
    /// Creates or gets a tag by name.
    /// </summary>
    /// <param name="name">The tag name.</param>
    /// <returns>The tag model.</returns>
    [HttpPost("ensure")]
    [ProducesResponseType(typeof(TagModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TagModel>> EnsureTag([FromQuery] string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return this.BadRequest("Tag name is required.");
        }

        try
        {
            var tag = await this.service.GetOrCreateByNameAsync(name);
            return this.Ok(tag);
        }
        catch (ArgumentException)
        {
            return this.BadRequest("Tag name cannot be empty.");
        }
    }
}
