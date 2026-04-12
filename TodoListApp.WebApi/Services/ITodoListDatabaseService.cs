using TodoListApp.WebApi.Models;

namespace TodoListApp.WebApi.Services;

public interface ITodoListDatabaseService
{
    Task<IEnumerable<TodoListModel>> GetAllAsync(string userId);

    Task<TodoListModel?> GetByIdAsync(int id);

    Task<TodoListModel> CreateAsync(TodoListModel model);

    Task UpdateAsync(TodoListModel model);

    Task DeleteAsync(int id);
}
