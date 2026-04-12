namespace TodoListApp.WebApi.Models.Entities;

public sealed class TodoItemEntity
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public DateTime CreatedDate { get; set; }

    public DateTime? DueDate { get; set; }

    public TaskItemStatus Status { get; set; }

    public string CreatorId { get; set; } = string.Empty;

    public string AssigneeId { get; set; } = string.Empty;

    public int TodoListId { get; set; }

    public TodoListEntity TodoList { get; set; } = null!;
}
