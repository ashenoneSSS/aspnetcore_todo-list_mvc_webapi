using Microsoft.EntityFrameworkCore;
using TodoListApp.WebApi.Data;
using TodoListApp.WebApi.Exceptions;
using TodoListApp.WebApi.Models;
using TodoListApp.WebApi.Models.Entities;

namespace TodoListApp.WebApi.Services;

/// <summary>
/// Database service implementation for todo list operations.
/// </summary>
public class TodoListDatabaseService : ITodoListDatabaseService
{
    private readonly TodoListDbContext context;

    /// <summary>
    /// Initializes a new instance of the <see cref="TodoListDatabaseService"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public TodoListDatabaseService(TodoListDbContext context)
    {
        this.context = context;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<TodoListModel>> GetAllAsync(string userId)
    {
        var entities = await this.context.TodoLists
            .Where(l => l.UserId == userId)
            .OrderByDescending(l => l.CreatedDate)
            .ToListAsync();

        return entities.Select(MapToModel);
    }

    /// <inheritdoc />
    public async Task<TodoListModel?> GetByIdAsync(int id)
    {
        var entity = await this.context.TodoLists.FindAsync(id);
        return entity == null ? null : MapToModel(entity);
    }

    /// <inheritdoc />
    public async Task<TodoListModel> CreateAsync(TodoListModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        var entity = new TodoListEntity
        {
            Title = model.Title,
            Description = model.Description,
            UserId = model.UserId,
            CreatedDate = model.CreatedDate,
        };

        this.context.TodoLists.Add(entity);
        await this.context.SaveChangesAsync();

        return MapToModel(entity);
    }

    /// <inheritdoc />
    public async Task UpdateAsync(TodoListModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        var entity = await this.context.TodoLists.FindAsync(model.Id)
            ?? throw new NotFoundException($"Todo list with id {model.Id} not found.");

        entity.Title = model.Title;
        entity.Description = model.Description;

        await this.context.SaveChangesAsync();
    }

    /// <inheritdoc />
    public async Task DeleteAsync(int id)
    {
        var entity = await this.context.TodoLists.FindAsync(id)
            ?? throw new NotFoundException($"Todo list with id {id} not found.");

        this.context.TodoLists.Remove(entity);
        await this.context.SaveChangesAsync();
    }

    private static TodoListModel MapToModel(TodoListEntity entity)
    {
        return new TodoListModel
        {
            Id = entity.Id,
            Title = entity.Title,
            Description = entity.Description,
            UserId = entity.UserId,
            CreatedDate = entity.CreatedDate,
        };
    }
}
