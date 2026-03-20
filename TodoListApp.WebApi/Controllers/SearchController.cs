using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoListApp.WebApi.Models;
using TodoListApp.WebApi.Services;

namespace TodoListApp.WebApi.Controllers;

/// <summary>
/// REST API controller for task search (Epic 4).
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SearchController : ControllerBase
{
    private readonly ITodoItemDatabaseService service;

    /// <summary>
    /// Initializes a new instance of the <see cref="SearchController"/> class.
    /// </summary>
    /// <param name="service">The todo item database service.</param>
    public SearchController(ITodoItemDatabaseService service)
    {
        this.service = service;
    }

    /// <summary>
    /// Searches for tasks by criteria.
    /// </summary>
    /// <param name="userId">The user identifier (required).</param>
    /// <param name="title">Optional: search by title (contains).</param>
    /// <param name="createdDateFrom">Optional: creation date from (yyyy-MM-dd).</param>
    /// <param name="createdDateTo">Optional: creation date to (yyyy-MM-dd).</param>
    /// <param name="dueDateFrom">Optional: due date from (yyyy-MM-dd).</param>
    /// <param name="dueDateTo">Optional: due date to (yyyy-MM-dd).</param>
    /// <param name="page">Page number (default 1).</param>
    /// <param name="pageSize">Page size (default 10, max 100).</param>
    /// <returns>Paginated list of tasks.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<TodoItemModel>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<TodoItemModel>>> Search(
        [FromQuery] string userId,
        [FromQuery] string? title,
        [FromQuery] DateTime? createdDateFrom,
        [FromQuery] DateTime? createdDateTo,
        [FromQuery] DateTime? dueDateFrom,
        [FromQuery] DateTime? dueDateTo,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            return this.BadRequest("userId is required.");
        }

        var items = await this.service.SearchAsync(
            userId,
            title,
            createdDateFrom,
            createdDateTo,
            dueDateFrom,
            dueDateTo,
            page,
            pageSize);

        return this.Ok(items);
    }
}
