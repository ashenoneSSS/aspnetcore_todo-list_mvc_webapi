namespace TodoListApp.WebApi.Models.Entities;

/// <summary>
/// Entity representing a todo list in the database.
/// </summary>
public sealed class TodoListEntity
{
    /// <summary>
    /// Gets or sets the unique identifier.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the list title.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the list description.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the creation date.
    /// </summary>
    public DateTime CreatedDate { get; set; }

    /// <summary>
    /// Gets or sets the user identifier who owns the list.
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Gets the navigation property for the todo items in this list.
    /// </summary>
    public ICollection<TodoItemEntity> Items { get; } = new List<TodoItemEntity>();
}
