using System.Net.Http.Json;
using System.Net.Sockets;
using System.Text.Json;
using TodoListApp.WebApp.Exceptions;
using TodoListApp.WebApp.Models;

namespace TodoListApp.WebApp.Services;

public class TodoListWebApiService : ITodoListWebApiService
{
    private static readonly JsonSerializerOptions JsonOptions = new ()
    {
        PropertyNameCaseInsensitive = true,
    };

    private readonly HttpClient httpClient;

    public TodoListWebApiService(HttpClient httpClient, IConfiguration configuration)
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

    public async Task<IEnumerable<TodoListWebApiModel>> GetAllAsync(string userId)
    {
        var uri = new Uri(
            this.httpClient.BaseAddress!,
            $"api/todolist?userId={Uri.EscapeDataString(userId)}");
        var response = await SendAsync(() => this.httpClient.GetAsync(uri));
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<IEnumerable<TodoListWebApiModel>>(JsonOptions);
        return result ?? Enumerable.Empty<TodoListWebApiModel>();
    }

    public async Task<TodoListWebApiModel?> GetByIdAsync(int id)
    {
        var uri = new Uri(this.httpClient.BaseAddress!, $"api/todolist/{id}");
        var response = await SendAsync(() => this.httpClient.GetAsync(uri));
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TodoListWebApiModel>(JsonOptions);
    }

    public Task CreateAsync(TodoListWebApiModel model)
    {
        ArgumentNullException.ThrowIfNull(model);
        return this.CreateCoreAsync(model);
    }

    public Task UpdateAsync(TodoListWebApiModel model)
    {
        ArgumentNullException.ThrowIfNull(model);
        return this.UpdateCoreAsync(model);
    }

    public Task DeleteAsync(int id)
    {
        return this.DeleteCoreAsync(id);
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

    private async Task CreateCoreAsync(TodoListWebApiModel model)
    {
        var response = await SendAsync(() => this.httpClient.PostAsJsonAsync("api/todolist", model, JsonOptions));
        response.EnsureSuccessStatusCode();
    }

    private async Task UpdateCoreAsync(TodoListWebApiModel model)
    {
        var response = await SendAsync(() => this.httpClient.PutAsJsonAsync($"api/todolist/{model.Id}", model, JsonOptions));
        response.EnsureSuccessStatusCode();
    }

    private async Task DeleteCoreAsync(int id)
    {
        var uri = new Uri(this.httpClient.BaseAddress!, $"api/todolist/{id}");
        var response = await SendAsync(() => this.httpClient.DeleteAsync(uri));
        response.EnsureSuccessStatusCode();
    }
}
