using System.ComponentModel.DataAnnotations;
using TodoListApp.WebApi.Models.Entities;

namespace TodoListApp.WebApi.Models;

public sealed class TodoItemModel
{
    public int Id { get; set; }

    [Required]
    [StringLength(200, MinimumLength = 1)]
    public string Title { get; set; } = string.Empty;

    [StringLength(2000)]
    public string? Description { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime? DueDate { get; set; }

    public TaskItemStatus Status { get; set; }

    [Required]
    [StringLength(450, MinimumLength = 1)]
    public string CreatorId { get; set; } = string.Empty;

    [Required]
    [StringLength(450, MinimumLength = 1)]
    public string AssigneeId { get; set; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int TodoListId { get; set; }

    // Tags removed (simplified app).
}
