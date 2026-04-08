using Microsoft.AspNetCore.Authentication;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using TodoListApp.WebApi.Data;
using TodoListApp.WebApi.Middleware;
using TodoListApp.WebApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<TodoListDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("TodoListDb")));

builder.Services.AddScoped<ITodoListDatabaseService, TodoListDatabaseService>();
builder.Services.AddScoped<ITodoItemDatabaseService, TodoItemDatabaseService>();

builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TodoListDbContext>();
    try
    {
        db.Database.Migrate();
    }
    catch (SqliteException ex) when (
        app.Environment.IsDevelopment() &&
        ex.SqliteErrorCode == 1 &&
        ex.Message.Contains("already exists", StringComparison.OrdinalIgnoreCase))
    {
        // Developer convenience: if someone manually cleared __EFMigrationsHistory without dropping tables,
        // EF tries to recreate existing tables and fails. In that case, we keep running with the existing schema.
        app.Logger.LogWarning(ex, "DB migration skipped because tables already exist (dev only).");
    }
}

app.UseHttpsRedirection();

app.UseMiddleware<ApiKeyAuthMiddleware>();
app.UseAuthorization();

app.MapControllers();

app.Run();
