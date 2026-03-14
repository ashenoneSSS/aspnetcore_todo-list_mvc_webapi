using System.ComponentModel.DataAnnotations;

namespace TodoListApp.WebApp.Models;

/// <summary>
/// Web API model for a todo item.
/// </summary>
public class TodoItemWebApiModel
{
    /// <summary>
    /// Gets or sets the unique identifier.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the item title.
    /// </summary>
    [Required]
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
    /// Gets or sets the task status (0=NotStarted, 1=InProgress, 2=Completed).
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// Gets or sets the assignee user identifier.
    /// </summary>
    public string AssigneeId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the parent todo list identifier.
    /// </summary>
    public int TodoListId { get; set; }

    /// <summary>
    /// Gets a value indicating whether the task is overdue.
    /// </summary>
    public bool IsOverdue => this.DueDate.HasValue && this.DueDate.Value < DateTime.Now && this.Status != 2;
}
