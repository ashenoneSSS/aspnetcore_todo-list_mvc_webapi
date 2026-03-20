using TodoListApp.WebApp.Models;

namespace TodoListApp.WebApp.Services;

/// <summary>
/// Web API service interface for todo items.
/// </summary>
public interface ITodoItemWebApiService
{
    /// <summary>
    /// Gets todo items by list identifier.
    /// </summary>
    /// <param name="listId">The todo list identifier.</param>
    /// <param name="page">Page number (1-based).</param>
    /// <param name="pageSize">Page size.</param>
    /// <returns>Collection of todo item models.</returns>
    Task<IEnumerable<TodoItemWebApiModel>> GetByListIdAsync(int listId, int page = 1, int pageSize = 10);

    /// <summary>
    /// Gets a todo item by identifier.
    /// </summary>
    /// <param name="id">The todo item identifier.</param>
    /// <returns>The todo item or null if not found.</returns>
    Task<TodoItemWebApiModel?> GetByIdAsync(int id);

    /// <summary>
    /// Gets todo items assigned to a user.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <returns>Collection of todo item models.</returns>
    Task<IEnumerable<TodoItemWebApiModel>> GetAssignedToUserAsync(string userId);

    /// <summary>
    /// Searches for tasks by criteria.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="title">Optional title search.</param>
    /// <param name="createdDateFrom">Optional creation date from.</param>
    /// <param name="createdDateTo">Optional creation date to.</param>
    /// <param name="dueDateFrom">Optional due date from.</param>
    /// <param name="dueDateTo">Optional due date to.</param>
    /// <param name="page">Page number.</param>
    /// <param name="pageSize">Page size.</param>
    /// <returns>Collection of todo item models.</returns>
    Task<IEnumerable<TodoItemWebApiModel>> SearchAsync(
        string userId,
        string? title,
        DateTime? createdDateFrom,
        DateTime? createdDateTo,
        DateTime? dueDateFrom,
        DateTime? dueDateTo,
        int page = 1,
        int pageSize = 20);

    /// <summary>
    /// Creates a new todo item.
    /// </summary>
    /// <param name="model">The todo item model.</param>
    /// <returns>A task representing the operation.</returns>
    Task CreateAsync(TodoItemWebApiModel model);

    /// <summary>
    /// Updates an existing todo item.
    /// </summary>
    /// <param name="model">The todo item model.</param>
    /// <returns>A task representing the operation.</returns>
    Task UpdateAsync(TodoItemWebApiModel model);

    /// <summary>
    /// Deletes a todo item.
    /// </summary>
    /// <param name="id">The todo item identifier.</param>
    /// <returns>A task representing the operation.</returns>
    Task DeleteAsync(int id);
}
