namespace TodoListApp.WebApi.Models.Entities;

/// <summary>
/// Entity representing a tag.
/// </summary>
public sealed class TagEntity
{
    /// <summary>
    /// Gets or sets the unique identifier.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the tag name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets the navigation property for todo items with this tag.
    /// </summary>
    public ICollection<TodoItemEntity> TodoItems { get; } = new List<TodoItemEntity>();
}
