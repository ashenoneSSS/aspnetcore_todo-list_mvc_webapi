using Microsoft.EntityFrameworkCore;
using TodoListApp.WebApi.Data;
using TodoListApp.WebApi.Exceptions;
using TodoListApp.WebApi.Models;
using TodoListApp.WebApi.Models.Entities;

namespace TodoListApp.WebApi.Services;

public class TodoListDatabaseService : ITodoListDatabaseService
{
    private readonly TodoListDbContext context;

    public TodoListDatabaseService(TodoListDbContext context)
    {
        this.context = context;
    }

    public async Task<IEnumerable<TodoListModel>> GetAllAsync(string userId)
    {
        var entities = await this.context.TodoLists
            .Where(l => l.UserId == userId)
            .OrderByDescending(l => l.CreatedDate)
            .ToListAsync();

        return entities.Select(MapToModel);
    }

    public async Task<TodoListModel?> GetByIdAsync(int id)
    {
        var entity = await this.context.TodoLists.FindAsync(id);
        return entity == null ? null : MapToModel(entity);
    }

    public Task<TodoListModel> CreateAsync(TodoListModel model)
    {
        ArgumentNullException.ThrowIfNull(model);
        return this.CreateCoreAsync(model);
    }

    public Task UpdateAsync(TodoListModel model)
    {
        ArgumentNullException.ThrowIfNull(model);
        return this.UpdateCoreAsync(model);
    }

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

    private async Task<TodoListModel> CreateCoreAsync(TodoListModel model)
    {
        var entity = new TodoListEntity
        {
            Title = model.Title,
            Description = model.Description ?? string.Empty,
            UserId = model.UserId,
            CreatedDate = model.CreatedDate,
        };

        this.context.TodoLists.Add(entity);
        await this.context.SaveChangesAsync();

        return MapToModel(entity);
    }

    private async Task UpdateCoreAsync(TodoListModel model)
    {
        var entity = await this.context.TodoLists.FindAsync(model.Id)
            ?? throw new NotFoundException($"Todo list with id {model.Id} not found.");

        entity.Title = model.Title;
        entity.Description = model.Description ?? string.Empty;

        await this.context.SaveChangesAsync();
    }
}
