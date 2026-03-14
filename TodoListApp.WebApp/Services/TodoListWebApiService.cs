using System.Net.Http.Json;
using System.Net.Sockets;
using System.Text.Json;
using TodoListApp.WebApp.Exceptions;
using TodoListApp.WebApp.Models;

namespace TodoListApp.WebApp.Services;

/// <summary>
/// Web API service implementation for todo lists.
/// </summary>
public class TodoListWebApiService : ITodoListWebApiService
{
    private static readonly JsonSerializerOptions JsonOptions = new ()
    {
        PropertyNameCaseInsensitive = true,
    };

    private readonly HttpClient httpClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="TodoListWebApiService"/> class.
    /// </summary>
    /// <param name="httpClient">The HTTP client.</param>
    /// <param name="configuration">The configuration.</param>
    public TodoListWebApiService(HttpClient httpClient, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        this.httpClient = httpClient;
        var baseUrl = configuration["WebApi:BaseUrl"] ?? "https://localhost:7001";
        var apiKey = configuration["WebApi:ApiKey"] ?? string.Empty;
        this.httpClient.BaseAddress = new Uri(baseUrl.TrimEnd('/') + "/");
        this.httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + apiKey);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<TodoListWebApiModel>> GetAllAsync(string userId)
    {
        var uri = new Uri(
            this.httpClient.BaseAddress!,
            $"api/todolist?userId={Uri.EscapeDataString(userId)}");
        var response = await this.SendAsync(() => this.httpClient.GetAsync(uri));
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<IEnumerable<TodoListWebApiModel>>(JsonOptions);
        return result ?? Enumerable.Empty<TodoListWebApiModel>();
    }

    /// <inheritdoc />
    public async Task<TodoListWebApiModel?> GetByIdAsync(int id)
    {
        var uri = new Uri(this.httpClient.BaseAddress!, $"api/todolist/{id}");
        var response = await this.SendAsync(() => this.httpClient.GetAsync(uri));
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
        var response = await this.SendAsync(() => this.httpClient.PostAsJsonAsync("api/todolist", model, JsonOptions));
        response.EnsureSuccessStatusCode();
    }

    /// <inheritdoc />
    public async Task UpdateAsync(TodoListWebApiModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        var response = await this.SendAsync(() => this.httpClient.PutAsJsonAsync($"api/todolist/{model.Id}", model, JsonOptions));
        response.EnsureSuccessStatusCode();
    }

    /// <inheritdoc />
    public async Task DeleteAsync(int id)
    {
        var uri = new Uri(this.httpClient.BaseAddress!, $"api/todolist/{id}");
        var response = await this.SendAsync(() => this.httpClient.DeleteAsync(uri));
        response.EnsureSuccessStatusCode();
    }

    private async Task<HttpResponseMessage> SendAsync(Func<Task<HttpResponseMessage>> send)
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
