using TodoListApp.WebApi.Models.Entities;

namespace TodoListApp.WebApi.Models;

/// <summary>
/// Data transfer object for a todo item.
/// </summary>
public class TodoItemModel
{
    /// <summary>
    /// Gets or sets the unique identifier.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the item title.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the item description.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the creation date.
    /// </summary>
    public DateTime CreatedDate { get; set; }

    /// <summary>
    /// Gets or sets the optional due date.
    /// </summary>
    public DateTime? DueDate { get; set; }

    /// <summary>
    /// Gets or sets the task status.
    /// </summary>
    public TaskItemStatus Status { get; set; }

    /// <summary>
    /// Gets or sets the assignee user identifier.
    /// </summary>
    public string AssigneeId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the parent todo list identifier.
    /// </summary>
    public int TodoListId { get; set; }
}
