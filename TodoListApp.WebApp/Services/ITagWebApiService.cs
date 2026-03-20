using TodoListApp.WebApp.Models;

namespace TodoListApp.WebApp.Services;

/// <summary>
/// Web API service interface for tags.
/// </summary>
public interface ITagWebApiService
{
    /// <summary>
    /// Gets all tags for the user.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <returns>Collection of tag models.</returns>
    Task<IEnumerable<TagWebApiModel>> GetAllAsync(string userId);

    /// <summary>
    /// Gets tasks that have a specific tag.
    /// </summary>
    /// <param name="tagId">The tag identifier.</param>
    /// <param name="userId">The user identifier.</param>
    /// <returns>Collection of todo item models.</returns>
    Task<IEnumerable<TodoItemWebApiModel>> GetTasksByTagAsync(int tagId, string userId);
}
