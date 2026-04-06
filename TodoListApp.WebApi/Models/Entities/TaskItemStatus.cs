namespace TodoListApp.WebApi.Models.Entities;

/// <summary>
/// Represents the status of a task item.
/// </summary>
public enum TaskItemStatus
{
    /// <summary>
    /// Task has not been started.
    /// </summary>
    NotStarted = 0,

    /// <summary>
    /// Task is in progress.
    /// </summary>
    InProgress = 1,

    /// <summary>
    /// Task is completed.
    /// </summary>
    Completed = 2,
}
