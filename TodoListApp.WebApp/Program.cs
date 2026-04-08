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
