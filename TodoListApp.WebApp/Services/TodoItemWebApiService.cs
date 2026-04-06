using System.Net.Http.Json;
using System.Text.Json;
using TodoListApp.WebApp.Models;

namespace TodoListApp.WebApp.Services;

/// <summary>
/// Web API service implementation for todo items.
/// </summary>
public class TodoItemWebApiService : ITodoItemWebApiService
{
    private readonly HttpClient _httpClient;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    /// <summary>
    /// Initializes a new instance of the <see cref="TodoItemWebApiService"/> class.
    /// </summary>
    /// <param name="httpClient">The HTTP client.</param>
    /// <param name="configuration">The configuration.</param>
    public TodoItemWebApiService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        var baseUrl = configuration["WebApi:BaseUrl"] ?? "https://localhost:7001";
        var apiKey = configuration["WebApi:ApiKey"] ?? string.Empty;
        _httpClient.BaseAddress = new Uri(baseUrl.TrimEnd('/') + "/");
        _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + apiKey);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<TodoItemWebApiModel>> GetByListIdAsync(int listId, int page = 1, int pageSize = 10)
    {
        var response = await _httpClient.GetAsync(
            $"api/todoitem?listId={listId}&page={page}&pageSize={pageSize}");
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<IEnumerable<TodoItemWebApiModel>>(JsonOptions);
        return result ?? Enumerable.Empty<TodoItemWebApiModel>();
    }

    /// <inheritdoc />
    public async Task<TodoItemWebApiModel?> GetByIdAsync(int id)
    {
        var response = await _httpClient.GetAsync($"api/todoitem/{id}");
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TodoItemWebApiModel>(JsonOptions);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<TodoItemWebApiModel>> GetAssignedToUserAsync(string userId)
    {
        var response = await _httpClient.GetAsync($"api/todoitem/assigned?userId={Uri.EscapeDataString(userId)}");
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<IEnumerable<TodoItemWebApiModel>>(JsonOptions);
        return result ?? Enumerable.Empty<TodoItemWebApiModel>();
    }

    /// <inheritdoc />
    public async Task CreateAsync(TodoItemWebApiModel model)
    {
        var response = await _httpClient.PostAsJsonAsync("api/todoitem", model, JsonOptions);
        response.EnsureSuccessStatusCode();
    }

    /// <inheritdoc />
    public async Task UpdateAsync(TodoItemWebApiModel model)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/todoitem/{model.Id}", model, JsonOptions);
        response.EnsureSuccessStatusCode();
    }

    /// <inheritdoc />
    public async Task DeleteAsync(int id)
    {
        var response = await _httpClient.DeleteAsync($"api/todoitem/{id}");
        response.EnsureSuccessStatusCode();
    }
}
