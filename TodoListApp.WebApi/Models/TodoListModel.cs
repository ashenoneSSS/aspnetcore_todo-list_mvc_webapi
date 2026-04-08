using System.ComponentModel.DataAnnotations;
using TodoListApp.WebApi.Models.Entities;

namespace TodoListApp.WebApi.Models;

/// <summary>
/// Data transfer object for a todo list.
/// </summary>
public sealed class TodoListModel
{
    /// <summary>
    /// Gets or sets the unique identifier.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the list title.
    /// </summary>
    [Required]
    [StringLength(200, MinimumLength = 1)]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the list description.
    /// </summary>
    [StringLength(2000)]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the user identifier who owns the list.
    /// </summary>
    [Required]
    [StringLength(450, MinimumLength = 1)]
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the creation date.
    /// </summary>
    public DateTime CreatedDate { get; set; }
}
