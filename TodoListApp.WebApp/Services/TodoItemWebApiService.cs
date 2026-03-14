using System.Net.Http.Json;
using System.Net.Sockets;
using System.Text.Json;
using TodoListApp.WebApp.Exceptions;
using TodoListApp.WebApp.Models;

namespace TodoListApp.WebApp.Services;

/// <summary>
/// Web API service implementation for todo items.
/// </summary>
public class TodoItemWebApiService : ITodoItemWebApiService
{
    private static readonly JsonSerializerOptions JsonOptions = new ()
    {
        PropertyNameCaseInsensitive = true,
    };

    private readonly HttpClient httpClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="TodoItemWebApiService"/> class.
    /// </summary>
    /// <param name="httpClient">The HTTP client.</param>
    /// <param name="configuration">The configuration.</param>
    public TodoItemWebApiService(HttpClient httpClient, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        this.httpClient = httpClient;
        var baseUrl = configuration["WebApi:BaseUrl"];
        if (string.IsNullOrEmpty(baseUrl))
        {
            throw new InvalidOperationException("WebApi:BaseUrl is not configured in appsettings.json.");
        }

        this.httpClient.BaseAddress = new Uri(baseUrl, UriKind.Absolute);
        var apiKey = configuration["WebApi:ApiKey"] ?? string.Empty;
        this.httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + apiKey);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<TodoItemWebApiModel>> GetByListIdAsync(int listId, int page = 1, int pageSize = 10)
    {
        var uri = new Uri(
            this.httpClient.BaseAddress!,
            $"api/todoitem?listId={listId}&page={page}&pageSize={pageSize}");
        var response = await SendAsync(() => this.httpClient.GetAsync(uri));
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<IEnumerable<TodoItemWebApiModel>>(JsonOptions);
        return result ?? Enumerable.Empty<TodoItemWebApiModel>();
    }

    /// <inheritdoc />
    public async Task<TodoItemWebApiModel?> GetByIdAsync(int id)
    {
        var uri = new Uri(this.httpClient.BaseAddress!, $"api/todoitem/{id}");
        var response = await SendAsync(() => this.httpClient.GetAsync(uri));
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
        var uri = new Uri(
            this.httpClient.BaseAddress!,
            $"api/todoitem/assigned?userId={Uri.EscapeDataString(userId)}");
        var response = await SendAsync(() => this.httpClient.GetAsync(uri));
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<IEnumerable<TodoItemWebApiModel>>(JsonOptions);
        return result ?? Enumerable.Empty<TodoItemWebApiModel>();
    }

    /// <inheritdoc />
    public Task CreateAsync(TodoItemWebApiModel model)
    {
        ArgumentNullException.ThrowIfNull(model);
        return this.CreateCoreAsync(model);
    }

    private async Task CreateCoreAsync(TodoItemWebApiModel model)
    {
        var response = await SendAsync(() => this.httpClient.PostAsJsonAsync("api/todoitem", model, JsonOptions));
        response.EnsureSuccessStatusCode();
    }

    /// <inheritdoc />
    public async Task UpdateAsync(TodoItemWebApiModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        var response = await SendAsync(() => this.httpClient.PutAsJsonAsync($"api/todoitem/{model.Id}", model, JsonOptions));
        response.EnsureSuccessStatusCode();
    }

    /// <inheritdoc />
    public async Task DeleteAsync(int id)
    {
        var uri = new Uri(this.httpClient.BaseAddress!, $"api/todoitem/{id}");
        var response = await SendAsync(() => this.httpClient.DeleteAsync(uri));
        response.EnsureSuccessStatusCode();
    }

    private static async Task<HttpResponseMessage> SendAsync(Func<Task<HttpResponseMessage>> send)
    {
        try
        {
            return await send();
        }
        catch (HttpRequestException ex)
        {
            throw new ApiUnavailableException("The Todo List API is not running or not reachable. Please start the WebApi project (TodoListApp.WebApi).", ex);
        }
        catch (SocketException ex)
        {
            throw new ApiUnavailableException("The Todo List API is not running or not reachable. Please start the WebApi project (TodoListApp.WebApi).", ex);
        }
    }
}
