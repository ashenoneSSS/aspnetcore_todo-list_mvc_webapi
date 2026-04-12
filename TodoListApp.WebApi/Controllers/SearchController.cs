using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoListApp.WebApi.Models;
using TodoListApp.WebApi.Services;

namespace TodoListApp.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SearchController : ControllerBase
{
    private readonly ITodoItemDatabaseService service;

    public SearchController(ITodoItemDatabaseService service)
    {
        this.service = service;
    }

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
