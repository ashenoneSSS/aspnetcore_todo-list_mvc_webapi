using System.ComponentModel.DataAnnotations;
using TodoListApp.WebApi.Models.Entities;

namespace TodoListApp.WebApi.Models;

/// <summary>
/// Data transfer object for a todo item.
/// </summary>
public sealed class TodoItemModel
{
    /// <summary>
    /// Gets or sets the unique identifier.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the item title.
    /// </summary>
    [Required]
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
    /// Gets or sets the task status.
    /// </summary>
    public TaskItemStatus Status { get; set; }

    /// <summary>
    /// Gets or sets the assignee user identifier.
    /// </summary>
    [Required]
    [StringLength(450, MinimumLength = 1)]
    public string AssigneeId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the parent todo list identifier.
    /// </summary>
    [Range(1, int.MaxValue)]
    public int TodoListId { get; set; }

    /// <summary>
    /// Gets or sets the tags on this task.
    /// </summary>
    public IList<TagModel> Tags { get; set; } = new List<TagModel>();
}
