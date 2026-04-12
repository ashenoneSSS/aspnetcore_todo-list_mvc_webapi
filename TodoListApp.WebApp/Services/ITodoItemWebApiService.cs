using TodoListApp.WebApp.Models;

namespace TodoListApp.WebApp.Services;

public interface ITodoItemWebApiService
{
    Task<IEnumerable<TodoItemWebApiModel>> GetByListIdAsync(int listId, int page = 1, int pageSize = 10);

    Task<TodoItemWebApiModel?> GetByIdAsync(int id);

    Task<IEnumerable<TodoItemWebApiModel>> GetAssignedToUserAsync(string userId);

    Task<IEnumerable<TodoItemWebApiModel>> GetAllForUserAsync(string userId);

    Task<IEnumerable<TodoItemWebApiModel>> SearchAsync(
        string userId,
        string? title,
        DateTime? createdDateFrom,
        DateTime? createdDateTo,
        DateTime? dueDateFrom,
        DateTime? dueDateTo,
        int page = 1,
        int pageSize = 20);

    Task CreateAsync(TodoItemWebApiModel model);

    Task UpdateAsync(TodoItemWebApiModel model);

    Task DeleteAsync(int id);
}
