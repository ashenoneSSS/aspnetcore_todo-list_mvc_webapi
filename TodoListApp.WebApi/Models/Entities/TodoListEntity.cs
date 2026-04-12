namespace TodoListApp.WebApi.Models.Entities;

public sealed class TodoListEntity
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public DateTime CreatedDate { get; set; }

    public string UserId { get; set; } = string.Empty;

    public ICollection<TodoItemEntity> Items { get; } = new List<TodoItemEntity>();
}
