using TodoListApp.WebApp.Models;

namespace TodoListApp.WebApp.Services;

public interface ITodoListWebApiService
{
    Task<IEnumerable<TodoListWebApiModel>> GetAllAsync(string userId);

    Task<TodoListWebApiModel?> GetByIdAsync(int id);

    Task CreateAsync(TodoListWebApiModel model);

    Task UpdateAsync(TodoListWebApiModel model);

    Task DeleteAsync(int id);
}
