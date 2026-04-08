using Microsoft.EntityFrameworkCore;
using TodoListApp.WebApp.Data;
using TodoListApp.WebApp.Filters;
using TodoListApp.WebApp.Models;
using TodoListApp.WebApp.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<ApiUnavailableExceptionFilter>();
});

builder.Services.AddDbContext<UsersDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("UsersDb")));

builder.Services.AddIdentity<ApplicationUser, Microsoft.AspNetCore.Identity.IdentityRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<UsersDbContext>();

builder.Services.AddHttpClient<ITodoListWebApiService, TodoListWebApiService>();
builder.Services.AddHttpClient<ITodoItemWebApiService, TodoItemWebApiService>();

var app = builder.Build();

// Keep local dev Identity DB schema in sync with the code.
using (var scope = app.Services.CreateScope())
{
    var usersDb = scope.ServiceProvider.GetRequiredService<UsersDbContext>();
    if (app.Environment.IsDevelopment())
    {
        // Auto-heal dev DB if Identity tables exist but migrations history is empty.
        var conn = usersDb.Database.GetDbConnection();
        var opened = false;
        try
        {
            if (conn.State != System.Data.ConnectionState.Open)
            {
                conn.Open();
                opened = true;
            }

            var hasAspNetRoles = false;
            var historyCount = 0;

            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText =
                    "SELECT 1 FROM sqlite_master WHERE type='table' AND name='AspNetRoles' LIMIT 1;";
                hasAspNetRoles = cmd.ExecuteScalar() != null;
            }

            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText =
                    "SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name='__EFMigrationsHistory';";
                var hasHistoryTable = Convert.ToInt32(cmd.ExecuteScalar()) > 0;
                if (hasHistoryTable)
                {
                    using var cmd2 = conn.CreateCommand();
                    cmd2.CommandText = "SELECT COUNT(*) FROM __EFMigrationsHistory;";
                    historyCount = Convert.ToInt32(cmd2.ExecuteScalar());
                }
                else
                {
                    historyCount = 0;
                }
            }

            if (hasAspNetRoles && historyCount == 0)
            {
                // Close connection before deleting file-backed SQLite DB.
                if (opened)
                {
                    conn.Close();
                    opened = false;
                }

                usersDb.Database.EnsureDeleted();
            }
        }
        finally
        {
            if (opened)
            {
                try { conn.Close(); } catch { /* ignore */ }
            }
        }
    }

    usersDb.Database.Migrate();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
