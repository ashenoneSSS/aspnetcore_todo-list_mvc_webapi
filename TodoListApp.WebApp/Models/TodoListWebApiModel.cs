using System.ComponentModel.DataAnnotations;

namespace TodoListApp.WebApp.Models;

/// <summary>
/// Web API model for a todo list.
/// </summary>
public class TodoListWebApiModel
{
    /// <summary>
    /// Gets or sets the unique identifier.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the list title.
    /// </summary>
    [Required]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the list description.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user identifier who owns the list.
    /// </summary>
    public string UserId { get; set; } = string.Empty;
}
