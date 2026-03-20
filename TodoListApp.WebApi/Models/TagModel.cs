namespace TodoListApp.WebApi.Models;

/// <summary>
/// Data transfer object for a tag.
/// </summary>
public sealed class TagModel
{
    /// <summary>
    /// Gets or sets the unique identifier.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the tag name.
    /// </summary>
    public string Name { get; set; } = string.Empty;
}
