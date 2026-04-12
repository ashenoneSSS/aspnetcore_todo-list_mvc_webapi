using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TodoListApp.WebApp.Models;

public class TodoItemWebApiModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Title is required")]
    [StringLength(200, MinimumLength = 1)]
    public string Title { get; set; } = string.Empty;

    [StringLength(2000)]
    public string? Description { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime? DueDate { get; set; }

    [Range(0, 2, ErrorMessage = "Status is required")]
    public int Status { get; set; }

    public string CreatorId { get; set; } = string.Empty;

    public string AssigneeId { get; set; } = string.Empty;

    [EmailAddress(ErrorMessage = "Assignee email is not a valid email address")]
    [JsonIgnore]
    public string? AssigneeEmail { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "TodoListId must be a positive integer")]
    public int TodoListId { get; set; }

    public bool IsOverdue => this.DueDate.HasValue && this.DueDate.Value < DateTime.Now && this.Status != 2;
}
