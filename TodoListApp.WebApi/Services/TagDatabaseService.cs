using Microsoft.EntityFrameworkCore;
using TodoListApp.WebApi.Data;
using TodoListApp.WebApi.Exceptions;
using TodoListApp.WebApi.Models;
using TodoListApp.WebApi.Models.Entities;

namespace TodoListApp.WebApi.Services;

/// <summary>
/// Database service implementation for tag operations.
/// </summary>
public class TagDatabaseService : ITagDatabaseService
{
    private readonly TodoListDbContext context;

    /// <summary>
    /// Initializes a new instance of the <see cref="TagDatabaseService"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public TagDatabaseService(TodoListDbContext context)
    {
        this.context = context;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<TagModel>> GetAllForUserAsync(string userId)
    {
        var tagIds = await this.context.TodoItems
            .Include(i => i.TodoList)
            .Include(i => i.Tags)
            .Where(i => i.TodoList.UserId == userId)
            .SelectMany(i => i.Tags.Select(t => t.Id))
            .Distinct()
            .ToListAsync();

        var tags = await this.context.Tags
            .Where(t => tagIds.Contains(t.Id))
            .OrderBy(t => t.Name)
            .Select(t => new TagModel { Id = t.Id, Name = t.Name })
            .ToListAsync();

        return tags;
    }

    /// <inheritdoc />
    public async Task<TagModel> GetOrCreateByNameAsync(string name)
    {
        ArgumentNullException.ThrowIfNull(name);
        var trimmed = name.Trim();
        if (string.IsNullOrEmpty(trimmed))
        {
            throw new ArgumentException("Tag name cannot be empty.", nameof(name));
        }

        var existing = await this.context.Tags.FirstOrDefaultAsync(t => t.Name == trimmed);
        if (existing != null)
        {
            return new TagModel { Id = existing.Id, Name = existing.Name };
        }

        var entity = new TagEntity { Name = trimmed };
        this.context.Tags.Add(entity);
        await this.context.SaveChangesAsync();
        return new TagModel { Id = entity.Id, Name = entity.Name };
    }

    /// <inheritdoc />
    public async Task<IEnumerable<TodoItemModel>> GetTasksByTagAsync(int tagId, string userId, int page = 1, int pageSize = 10)
    {
        var skip = (Math.Max(1, page) - 1) * Math.Clamp(pageSize, 1, 100);
        var entities = await this.context.TodoItems
            .Include(i => i.TodoList)
            .Include(i => i.Tags)
            .Where(i => i.Tags.Any(t => t.Id == tagId) && i.TodoList.UserId == userId)
            .OrderBy(i => i.DueDate)
            .ThenBy(i => i.Title)
            .Skip(skip)
            .Take(Math.Clamp(pageSize, 1, 100))
            .ToListAsync();

        return entities.Select(e => new TodoItemModel
        {
            Id = e.Id,
            Title = e.Title,
            Description = e.Description,
            CreatedDate = e.CreatedDate,
            DueDate = e.DueDate,
            Status = e.Status,
            AssigneeId = e.AssigneeId,
            TodoListId = e.TodoListId,
            Tags = e.Tags.Select(t => new TagModel { Id = t.Id, Name = t.Name }).ToList(),
        });
    }

    /// <inheritdoc />
    public async Task AddTagToTaskAsync(int todoItemId, int tagId)
    {
        var item = await this.context.TodoItems
            .Include(i => i.Tags)
            .FirstOrDefaultAsync(i => i.Id == todoItemId)
            ?? throw new NotFoundException($"Todo item {todoItemId} not found.");

        var tag = await this.context.Tags.FindAsync(tagId)
            ?? throw new NotFoundException($"Tag {tagId} not found.");

        if (item.Tags.Any(t => t.Id == tagId))
        {
            return;
        }

        item.Tags.Add(tag);
        await this.context.SaveChangesAsync();
    }

    /// <inheritdoc />
    public async Task RemoveTagFromTaskAsync(int todoItemId, int tagId)
    {
        var item = await this.context.TodoItems
            .Include(i => i.Tags)
            .FirstOrDefaultAsync(i => i.Id == todoItemId)
            ?? throw new NotFoundException($"Todo item {todoItemId} not found.");

        var tag = item.Tags.FirstOrDefault(t => t.Id == tagId);
        if (tag != null)
        {
            item.Tags.Remove(tag);
            await this.context.SaveChangesAsync();
        }
    }
}
