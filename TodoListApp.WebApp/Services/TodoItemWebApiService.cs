using System.Net.Http.Json;
using System.Text.Json;
using TodoListApp.WebApp.Exceptions;
using TodoListApp.WebApp.Models;

namespace TodoListApp.WebApp.Services;

public class TodoItemWebApiService : ITodoItemWebApiService
{
    private static readonly JsonSerializerOptions JsonOptions = new ()
    {
        PropertyNameCaseInsensitive = true,
    };

    private readonly HttpClient httpClient;

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

    public async Task<IEnumerable<TodoItemWebApiModel>> GetByListIdAsync(int listId, int page = 1, int pageSize = 10)
    {
        var uri = new Uri(
            this.httpClient.BaseAddress!,
            $"api/todoitem?listId={listId}&page={page}&pageSize={pageSize}");
        var response = await SendAsync(() => this.httpClient.GetAsync(uri));
        _ = response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<IEnumerable<TodoItemWebApiModel>>(JsonOptions);
        return result ?? Enumerable.Empty<TodoItemWebApiModel>();
    }

    public async Task<TodoItemWebApiModel?> GetByIdAsync(int id)
    {
        var uri = new Uri(this.httpClient.BaseAddress!, $"api/todoitem/{id}");
        var response = await SendAsync(() => this.httpClient.GetAsync(uri));
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }

        _ = response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TodoItemWebApiModel>(JsonOptions);
    }

    public async Task<IEnumerable<TodoItemWebApiModel>> GetAssignedToUserAsync(string userId)
    {
        var uri = new Uri(
            this.httpClient.BaseAddress!,
            $"api/todoitem/assigned?userId={Uri.EscapeDataString(userId)}");
        var response = await SendAsync(() => this.httpClient.GetAsync(uri));
        _ = response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<IEnumerable<TodoItemWebApiModel>>(JsonOptions);
        return result ?? Enumerable.Empty<TodoItemWebApiModel>();
    }

    public async Task<IEnumerable<TodoItemWebApiModel>> GetAllForUserAsync(string userId)
    {
        var uri = new Uri(
            this.httpClient.BaseAddress!,
            $"api/todoitem/mine?userId={Uri.EscapeDataString(userId)}");
        var response = await SendAsync(() => this.httpClient.GetAsync(uri));
        _ = response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<IEnumerable<TodoItemWebApiModel>>(JsonOptions);
        return result ?? Enumerable.Empty<TodoItemWebApiModel>();
    }

    public async Task<IEnumerable<TodoItemWebApiModel>> SearchAsync(
        string userId,
        string? title,
        DateTime? createdDateFrom,
        DateTime? createdDateTo,
        DateTime? dueDateFrom,
        DateTime? dueDateTo,
        int page = 1,
        int pageSize = 20)
    {
        var query = new List<string> { $"userId={Uri.EscapeDataString(userId)}", $"page={page}", $"pageSize={pageSize}" };
        if (!string.IsNullOrWhiteSpace(title))
        {
            query.Add($"title={Uri.EscapeDataString(title.Trim())}");
        }

        if (createdDateFrom.HasValue)
        {
            query.Add($"createdDateFrom={createdDateFrom:yyyy-MM-dd}");
        }

        if (createdDateTo.HasValue)
        {
            query.Add($"createdDateTo={createdDateTo:yyyy-MM-dd}");
        }

        if (dueDateFrom.HasValue)
        {
            query.Add($"dueDateFrom={dueDateFrom:yyyy-MM-dd}");
        }

        if (dueDateTo.HasValue)
        {
            query.Add($"dueDateTo={dueDateTo:yyyy-MM-dd}");
        }

        var uri = new Uri(
            this.httpClient.BaseAddress!,
            "api/search?" + string.Join("&", query));
        var response = await SendAsync(() => this.httpClient.GetAsync(uri));
        _ = response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<IEnumerable<TodoItemWebApiModel>>(JsonOptions);
        return result ?? Enumerable.Empty<TodoItemWebApiModel>();
    }

    public Task CreateAsync(TodoItemWebApiModel model)
    {
        ArgumentNullException.ThrowIfNull(model);
        return this.CreateCoreAsync(model);
    }

    public Task UpdateAsync(TodoItemWebApiModel model)
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
        catch (System.Net.Sockets.SocketException ex)
        {
            throw new ApiUnavailableException("The Todo List API is not running or not reachable. Please start the WebApi project (TodoListApp.WebApi).", ex);
        }
    }

    private async Task CreateCoreAsync(TodoItemWebApiModel model)
    {
        var response = await SendAsync(() => this.httpClient.PostAsJsonAsync("api/todoitem", model, JsonOptions));
        _ = response.EnsureSuccessStatusCode();
    }

    private async Task UpdateCoreAsync(TodoItemWebApiModel model)
    {
        var response = await SendAsync(() => this.httpClient.PutAsJsonAsync($"api/todoitem/{model.Id}", model, JsonOptions));
        _ = response.EnsureSuccessStatusCode();
    }

    private async Task DeleteCoreAsync(int id)
    {
        var uri = new Uri(this.httpClient.BaseAddress!, $"api/todoitem/{id}");
        var response = await SendAsync(() => this.httpClient.DeleteAsync(uri));
        _ = response.EnsureSuccessStatusCode();
    }
}
