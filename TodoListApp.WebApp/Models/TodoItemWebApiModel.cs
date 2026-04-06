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
    [Required(ErrorMessage = "Title is required")]
    [StringLength(200, MinimumLength = 1)]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the item description.
    /// </summary>
    [StringLength(2000)]
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
    [Range(0, 2, ErrorMessage = "Status is required")]
    public int Status { get; set; }

    /// <summary>
    /// Gets or sets the assignee user identifier.
    /// </summary>
    public string AssigneeId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the parent todo list identifier.
    /// </summary>
    [Range(1, int.MaxValue, ErrorMessage = "TodoListId must be a positive integer")]
    public int TodoListId { get; set; }

    /// <summary>
    /// Gets a value indicating whether the task is overdue.
    /// </summary>
    public bool IsOverdue => this.DueDate.HasValue && this.DueDate.Value < DateTime.Now && this.Status != 2;

    /// <summary>
    /// Gets or sets the tags on this task.
    /// </summary>
    public IList<TagWebApiModel> Tags { get; set; } = new List<TagWebApiModel>();
}

/// <summary>
/// Web API model for a tag.
/// </summary>
public class TagWebApiModel
{
    /// <summary>
    /// Gets or sets the tag id.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the tag name.
    /// </summary>
    public string Name { get; set; } = string.Empty;
}
