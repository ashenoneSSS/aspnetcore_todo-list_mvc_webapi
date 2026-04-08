using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
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

// Keep local dev DB schema in sync with the code.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TodoListDbContext>();
    // #region agent log
    static void DebugLog(string hypothesisId, string message, object? data)
    {
        try
        {
            var payload = new
            {
                sessionId = "c23ceb",
                runId = "pre-fix",
                hypothesisId,
                location = "TodoListApp.WebApi/Program.cs:migrate",
                message,
                data,
                timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            };

            var line = JsonSerializer.Serialize(payload);
            File.AppendAllText(
                @"C:\Users\ne_kazual\Desktop\EPAM .NET\Epam_Tasks\26_03_13_Capstone-Project_todo-list-app\debug-c23ceb.log",
                line + Environment.NewLine);
        }
        catch
        {
            // never break app startup because of debug logging
        }
    }
    // #endregion

    DebugLog("H1", "Starting DB migration", new
    {
        environment = app.Environment.EnvironmentName,
        currentDirectory = Directory.GetCurrentDirectory(),
        baseDirectory = AppContext.BaseDirectory,
        connectionString = db.Database.GetConnectionString(),
        dataSource = db.Database.GetDbConnection().DataSource,
        provider = db.Database.ProviderName,
    });

    var todoListsTableExists = false;
    var historyRowCount = -1;
    var openedConnectionForInspection = false;

    try
    {
        var conn = db.Database.GetDbConnection();
        if (conn.State != System.Data.ConnectionState.Open)
        {
            conn.Open();
            openedConnectionForInspection = true;
        }

        try
        {
            var applied = db.Database.GetAppliedMigrations().ToArray();
            var pending = db.Database.GetPendingMigrations().ToArray();
            DebugLog("H3", "EF migrations state", new
            {
                appliedCount = applied.Length,
                pendingCount = pending.Length,
                applied,
                pending,
            });
        }
        catch (Exception ex)
        {
            DebugLog("H3", "Failed to get EF migrations state", new { ex = ex.GetType().FullName, ex.Message });
        }

        using (var cmd = conn.CreateCommand())
        {
            cmd.CommandText =
                "SELECT name, type FROM sqlite_master WHERE type IN ('table','view') ORDER BY type, name;";
            using var reader = cmd.ExecuteReader();
            var objects = new List<object>();
            while (reader.Read())
            {
                var name = reader.GetString(0);
                var type = reader.GetString(1);
                if (type == "table" && name == "TodoLists")
                {
                    todoListsTableExists = true;
                }
                objects.Add(new { name, type });
            }
            DebugLog("H2", "SQLite objects before migrate", new { count = objects.Count, objects });
        }

        using (var cmd = conn.CreateCommand())
        {
            cmd.CommandText =
                "SELECT name FROM sqlite_master WHERE type='table' AND name='__EFMigrationsHistory' LIMIT 1;";
            var hasHistory = cmd.ExecuteScalar() != null;
            DebugLog("H3", "Has __EFMigrationsHistory table", new { hasHistory });
        }

        using (var cmd = conn.CreateCommand())
        {
            cmd.CommandText =
                "SELECT MigrationId, ProductVersion FROM __EFMigrationsHistory ORDER BY MigrationId;";
            using var reader = cmd.ExecuteReader();
            var rows = new List<object>();
            while (reader.Read())
            {
                rows.Add(new { migrationId = reader.GetString(0), productVersion = reader.GetString(1) });
            }
            historyRowCount = rows.Count;
            DebugLog("H3", "__EFMigrationsHistory rows", new { count = rows.Count, rows });
        }

        if (openedConnectionForInspection)
        {
            try
            {
                conn.Close();
                DebugLog("H2", "Closed inspection connection", new { state = conn.State.ToString() });
            }
            catch (Exception ex)
            {
                DebugLog("H2", "Failed to close inspection connection", new { ex = ex.GetType().FullName, ex.Message });
            }
        }
    }
    catch (Exception ex)
    {
        DebugLog("H4", "Failed pre-migrate inspection", new { ex = ex.GetType().FullName, ex.Message });
    }

    // Auto-heal dev DB when schema exists but migration history is empty.
    // This happens if DB was created outside EF migrations (manual script / EnsureCreated),
    // and then EF migrations run later and try to recreate existing tables.
    if (app.Environment.IsDevelopment()
        && todoListsTableExists
        && historyRowCount == 0)
    {
        DebugLog("H6", "Detected inconsistent dev DB (tables exist, no migration history). Recreating DB.", new
        {
            todoListsTableExists,
            historyRowCount,
            connectionState = db.Database.GetDbConnection().State.ToString(),
        });

        try
        {
            db.Database.EnsureDeleted();
            DebugLog("H6", "EnsureDeleted succeeded", null);
        }
        catch (Exception ex)
        {
            DebugLog("H6", "EnsureDeleted failed", new { ex = ex.GetType().FullName, ex.Message });
            throw;
        }
    }

    try
    {
        db.Database.Migrate();
        DebugLog("H1", "DB migration succeeded", null);
    }
    catch (Exception ex)
    {
        DebugLog("H5", "DB migration failed", new
        {
            ex = ex.GetType().FullName,
            ex.Message,
            inner = ex.InnerException == null ? null : new { ex.InnerException.GetType().FullName, ex.InnerException.Message }
        });
        throw;
    }
}

app.UseHttpsRedirection();

app.UseMiddleware<ApiKeyAuthMiddleware>();
app.UseAuthorization();

app.MapControllers();

app.Run();
