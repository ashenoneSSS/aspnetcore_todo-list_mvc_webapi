using TodoListApp.WebApi.Models;

namespace TodoListApp.WebApi.Services;

/// <summary>
/// Service interface for todo list database operations.
/// </summary>
public interface ITodoListDatabaseService
{
    /// <summary>
    /// Gets all todo lists for a user.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <returns>Collection of todo list models.</returns>
    Task<IEnumerable<TodoListModel>> GetAllAsync(string userId);

    /// <summary>
    /// Gets a todo list by identifier.
    /// </summary>
    /// <param name="id">The todo list identifier.</param>
    /// <returns>The todo list model or null if not found.</returns>
    Task<TodoListModel?> GetByIdAsync(int id);

    /// <summary>
    /// Creates a new todo list.
    /// </summary>
    /// <param name="model">The todo list model.</param>
    /// <returns>The created todo list with assigned Id.</returns>
    Task<TodoListModel> CreateAsync(TodoListModel model);

    /// <summary>
    /// Updates an existing todo list.
    /// </summary>
    /// <param name="model">The todo list model.</param>
    /// <returns>A task representing the operation.</returns>
    Task UpdateAsync(TodoListModel model);

    /// <summary>
    /// Deletes a todo list by identifier.
    /// </summary>
    /// <param name="id">The todo list identifier.</param>
    /// <returns>A task representing the operation.</returns>
    Task DeleteAsync(int id);
}
