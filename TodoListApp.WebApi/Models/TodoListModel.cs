using TodoListApp.WebApi.Models.Entities;

namespace TodoListApp.WebApi.Models;

/// <summary>
/// Data transfer object for a todo list.
/// </summary>
public class TodoListModel
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
    /// Gets or sets the user identifier who owns the list.
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the creation date.
    /// </summary>
    public DateTime CreatedDate { get; set; }
}
