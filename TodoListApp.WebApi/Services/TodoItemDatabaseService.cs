using Microsoft.EntityFrameworkCore;
using TodoListApp.WebApi.Data;
using TodoListApp.WebApi.Exceptions;
using TodoListApp.WebApi.Models;
using TodoListApp.WebApi.Models.Entities;

namespace TodoListApp.WebApi.Services;

/// <summary>
/// Database service implementation for todo item operations.
/// </summary>
public class TodoItemDatabaseService : ITodoItemDatabaseService
{
    private readonly TodoListDbContext context;

    /// <summary>
    /// Initializes a new instance of the <see cref="TodoItemDatabaseService"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public TodoItemDatabaseService(TodoListDbContext context)
    {
        this.context = context;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<TodoItemModel>> GetByListIdAsync(int listId, int page = 1, int pageSize = 10)
    {
        var skip = (page - 1) * pageSize;

        var entities = await this.context.TodoItems
            .Where(i => i.TodoListId == listId)
            .OrderBy(i => i.CreatedDate)
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync();

        return entities.Select(MapToModel);
    }

    /// <inheritdoc />
    public async Task<TodoItemModel?> GetByIdAsync(int id)
    {
        var entity = await this.context.TodoItems.FindAsync(id);
        return entity == null ? null : MapToModel(entity);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<TodoItemModel>> GetAssignedToUserAsync(string userId)
    {
        var entities = await this.context.TodoItems
            .Where(i => i.AssigneeId == userId)
            .OrderBy(i => i.DueDate)
            .ThenBy(i => i.Title)
            .ToListAsync();

        return entities.Select(MapToModel);
    }

    /// <inheritdoc />
    public Task<TodoItemModel> CreateAsync(TodoItemModel model)
    {
        ArgumentNullException.ThrowIfNull(model);
        return this.CreateCoreAsync(model);
    }

    /// <inheritdoc />
    public Task UpdateAsync(TodoItemModel model)
    {
        ArgumentNullException.ThrowIfNull(model);
        return this.UpdateCoreAsync(model);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(int id)
    {
        var entity = await this.context.TodoItems.FindAsync(id)
            ?? throw new NotFoundException($"Todo item with id {id} not found.");

        this.context.TodoItems.Remove(entity);
        await this.context.SaveChangesAsync();
    }

    private static TodoItemModel MapToModel(TodoItemEntity entity)
    {
        return new TodoItemModel
        {
            Id = entity.Id,
            Title = entity.Title,
            Description = entity.Description,
            CreatedDate = entity.CreatedDate,
            DueDate = entity.DueDate,
            Status = entity.Status,
            AssigneeId = entity.AssigneeId,
            TodoListId = entity.TodoListId,
        };
    }

    private async Task<TodoItemModel> CreateCoreAsync(TodoItemModel model)
    {
        var entity = new TodoItemEntity
        {
            Title = model.Title,
            Description = model.Description,
            CreatedDate = model.CreatedDate,
            DueDate = model.DueDate,
            Status = model.Status,
            AssigneeId = model.AssigneeId,
            TodoListId = model.TodoListId,
        };

        this.context.TodoItems.Add(entity);
        await this.context.SaveChangesAsync();

        return MapToModel(entity);
    }

    private async Task UpdateCoreAsync(TodoItemModel model)
    {
        var entity = await this.context.TodoItems.FindAsync(model.Id)
            ?? throw new NotFoundException($"Todo item with id {model.Id} not found.");

        entity.Title = model.Title;
        entity.Description = model.Description;
        entity.DueDate = model.DueDate;
        entity.Status = model.Status;
        entity.AssigneeId = model.AssigneeId;

        await this.context.SaveChangesAsync();
    }
}
