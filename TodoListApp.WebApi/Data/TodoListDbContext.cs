using Microsoft.EntityFrameworkCore;
using TodoListApp.WebApi.Models.Entities;

namespace TodoListApp.WebApi.Data;

/// <summary>
/// Database context for the TodoList application.
/// </summary>
public class TodoListDbContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TodoListDbContext"/> class.
    /// </summary>
    /// <param name="options">The options to be used by the context.</param>
    public TodoListDbContext(DbContextOptions<TodoListDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Gets or sets the todo lists DbSet.
    /// </summary>
    public DbSet<TodoListEntity> TodoLists { get; set; }

    /// <summary>
    /// Gets or sets the todo items DbSet.
    /// </summary>
    public DbSet<TodoItemEntity> TodoItems { get; set; }

    /// <summary>
    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<TodoItemEntity>()
            .HasOne(t => t.TodoList)
            .WithMany(l => l.Items)
            .HasForeignKey(t => t.TodoListId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
