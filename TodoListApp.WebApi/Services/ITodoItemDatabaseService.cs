using TodoListApp.WebApi.Models;

namespace TodoListApp.WebApi.Services;

/// <summary>
/// Service interface for todo item database operations.
/// </summary>
public interface ITodoItemDatabaseService
{
    /// <summary>
    /// Gets todo items by list identifier with pagination.
    /// </summary>
    /// <param name="listId">The todo list identifier.</param>
    /// <param name="page">The page number (1-based).</param>
    /// <param name="pageSize">The page size.</param>
    /// <returns>Collection of todo item models.</returns>
    Task<IEnumerable<TodoItemModel>> GetByListIdAsync(int listId, int page = 1, int pageSize = 10);

    /// <summary>
    /// Gets a todo item by identifier.
    /// </summary>
    /// <param name="id">The todo item identifier.</param>
    /// <returns>The todo item model or null if not found.</returns>
    Task<TodoItemModel?> GetByIdAsync(int id);

    /// <summary>
    /// Gets todo items assigned to a user.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <returns>Collection of todo item models.</returns>
    Task<IEnumerable<TodoItemModel>> GetAssignedToUserAsync(string userId);

    /// <summary>
    /// Creates a new todo item.
    /// </summary>
    /// <param name="model">The todo item model.</param>
    /// <returns>The created todo item with assigned Id.</returns>
    Task<TodoItemModel> CreateAsync(TodoItemModel model);

    /// <summary>
    /// Updates an existing todo item.
    /// </summary>
    /// <param name="model">The todo item model.</param>
    /// <returns>A task representing the operation.</returns>
    Task UpdateAsync(TodoItemModel model);

    /// <summary>
    /// Deletes a todo item by identifier.
    /// </summary>
    /// <param name="id">The todo item identifier.</param>
    /// <returns>A task representing the operation.</returns>
    Task DeleteAsync(int id);
}
