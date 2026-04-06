using TodoListApp.WebApp.Models;

namespace TodoListApp.WebApp.Services;

/// <summary>
/// Web API service interface for todo lists.
/// </summary>
public interface ITodoListWebApiService
{
    /// <summary>
    /// Gets all todo lists for a user.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <returns>Collection of todo list models.</returns>
    Task<IEnumerable<TodoListWebApiModel>> GetAllAsync(string userId);

    /// <summary>
    /// Gets a todo list by identifier.
    /// </summary>
    /// <param name="id">The todo list identifier.</param>
    /// <returns>The todo list or null if not found.</returns>
    Task<TodoListWebApiModel?> GetByIdAsync(int id);

    /// <summary>
    /// Creates a new todo list.
    /// </summary>
    /// <param name="model">The todo list model.</param>
    /// <returns>A task representing the operation.</returns>
    Task CreateAsync(TodoListWebApiModel model);

    /// <summary>
    /// Updates an existing todo list.
    /// </summary>
    /// <param name="model">The todo list model.</param>
    /// <returns>A task representing the operation.</returns>
    Task UpdateAsync(TodoListWebApiModel model);

    /// <summary>
    /// Deletes a todo list.
    /// </summary>
    /// <param name="id">The todo list identifier.</param>
    /// <returns>A task representing the operation.</returns>
    Task DeleteAsync(int id);
}
