using System.Net.Http.Json;
using System.Text.Json;
using TodoListApp.WebApp.Models;

namespace TodoListApp.WebApp.Services;

/// <summary>
/// Web API service implementation for todo lists.
/// </summary>
public class TodoListWebApiService : ITodoListWebApiService
{
    private readonly HttpClient _httpClient;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    /// <summary>
    /// Initializes a new instance of the <see cref="TodoListWebApiService"/> class.
    /// </summary>
    /// <param name="httpClient">The HTTP client.</param>
    /// <param name="configuration">The configuration.</param>
    public TodoListWebApiService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        var baseUrl = configuration["WebApi:BaseUrl"] ?? "https://localhost:7001";
        var apiKey = configuration["WebApi:ApiKey"] ?? string.Empty;
        _httpClient.BaseAddress = new Uri(baseUrl.TrimEnd('/') + "/");
        _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + apiKey);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<TodoListWebApiModel>> GetAllAsync(string userId)
    {
        var response = await _httpClient.GetAsync($"api/todolist?userId={Uri.EscapeDataString(userId)}");
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<IEnumerable<TodoListWebApiModel>>(JsonOptions);
        return result ?? Enumerable.Empty<TodoListWebApiModel>();
    }

    /// <inheritdoc />
    public async Task<TodoListWebApiModel?> GetByIdAsync(int id)
    {
        var response = await _httpClient.GetAsync($"api/todolist/{id}");
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TodoListWebApiModel>(JsonOptions);
    }

    /// <inheritdoc />
    public async Task CreateAsync(TodoListWebApiModel model)
    {
        var response = await _httpClient.PostAsJsonAsync("api/todolist", model, JsonOptions);
        response.EnsureSuccessStatusCode();
    }

    /// <inheritdoc />
    public async Task UpdateAsync(TodoListWebApiModel model)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/todolist/{model.Id}", model, JsonOptions);
        response.EnsureSuccessStatusCode();
    }

    /// <inheritdoc />
    public async Task DeleteAsync(int id)
    {
        var response = await _httpClient.DeleteAsync($"api/todolist/{id}");
        response.EnsureSuccessStatusCode();
    }
}
