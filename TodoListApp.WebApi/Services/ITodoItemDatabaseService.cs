using TodoListApp.WebApi.Models;

namespace TodoListApp.WebApi.Services;

public interface ITodoItemDatabaseService
{
    Task<IEnumerable<TodoItemModel>> GetByListIdAsync(int listId, int page = 1, int pageSize = 10);

    Task<TodoItemModel?> GetByIdAsync(int id);

    Task<IEnumerable<TodoItemModel>> GetAssignedToUserAsync(string userId);

    Task<IEnumerable<TodoItemModel>> GetAllForUserAsync(string userId);

    Task<IEnumerable<TodoItemModel>> SearchAsync(
        string userId,
        string? title,
        DateTime? createdDateFrom,
        DateTime? createdDateTo,
        DateTime? dueDateFrom,
        DateTime? dueDateTo,
        int page = 1,
        int pageSize = 10);

    Task<TodoItemModel> CreateAsync(TodoItemModel model);

    Task UpdateAsync(TodoItemModel model);

    Task DeleteAsync(int id);
}
