using TodoListApp.WebApi.Models;

namespace TodoListApp.WebApi.Services;

/// <summary>
/// Service interface for tag database operations.
/// </summary>
public interface ITagDatabaseService
{
    /// <summary>
    /// Gets all tags for tasks in lists owned by the user.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <returns>Collection of tag models.</returns>
    Task<IEnumerable<TagModel>> GetAllForUserAsync(string userId);

    /// <summary>
    /// Gets or creates a tag by name.
    /// </summary>
    /// <param name="name">The tag name.</param>
    /// <returns>The tag model.</returns>
    Task<TagModel> GetOrCreateByNameAsync(string name);

    /// <summary>
    /// Gets tasks that have a specific tag.
    /// </summary>
    /// <param name="tagId">The tag identifier.</param>
    /// <param name="userId">The user identifier (for access scope).</param>
    /// <param name="page">Page number.</param>
    /// <param name="pageSize">Page size.</param>
    /// <returns>Collection of todo item models.</returns>
    Task<IEnumerable<TodoItemModel>> GetTasksByTagAsync(int tagId, string userId, int page = 1, int pageSize = 10);

    /// <summary>
    /// Adds a tag to a task.
    /// </summary>
    /// <param name="todoItemId">The todo item identifier.</param>
    /// <param name="tagId">The tag identifier.</param>
    /// <returns>A task representing the operation.</returns>
    Task AddTagToTaskAsync(int todoItemId, int tagId);

    /// <summary>
    /// Removes a tag from a task.
    /// </summary>
    /// <param name="todoItemId">The todo item identifier.</param>
    /// <param name="tagId">The tag identifier.</param>
    /// <returns>A task representing the operation.</returns>
    Task RemoveTagFromTaskAsync(int todoItemId, int tagId);
}
