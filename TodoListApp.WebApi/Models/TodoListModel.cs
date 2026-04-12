using System.ComponentModel.DataAnnotations;
using TodoListApp.WebApi.Models.Entities;

namespace TodoListApp.WebApi.Models;

public sealed class TodoListModel
{
    public int Id { get; set; }

    [Required]
    [StringLength(200, MinimumLength = 1)]
    public string Title { get; set; } = string.Empty;

    [StringLength(2000)]
    public string? Description { get; set; }

    [Required]
    [StringLength(450, MinimumLength = 1)]
    public string UserId { get; set; } = string.Empty;

    public DateTime CreatedDate { get; set; }
}
