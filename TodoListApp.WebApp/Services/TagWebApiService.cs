using System.Net.Http.Json;
using System.Text.Json;
using TodoListApp.WebApp.Exceptions;
using TodoListApp.WebApp.Models;

namespace TodoListApp.WebApp.Services;

/// <summary>
/// Web API service implementation for tags.
/// </summary>
public class TagWebApiService : ITagWebApiService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    private readonly HttpClient httpClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="TagWebApiService"/> class.
    /// </summary>
    /// <param name="httpClient">The HTTP client.</param>
    /// <param name="configuration">The configuration.</param>
    public TagWebApiService(HttpClient httpClient, IConfiguration configuration)
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
    public async Task<IEnumerable<TagWebApiModel>> GetAllAsync(string userId)
    {
        var uri = new Uri(
            this.httpClient.BaseAddress!,
            $"api/tag?userId={Uri.EscapeDataString(userId)}");
        var response = await SendAsync(() => this.httpClient.GetAsync(uri));
        _ = response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<IEnumerable<TagWebApiModel>>(JsonOptions);
        return result ?? Enumerable.Empty<TagWebApiModel>();
    }

    /// <inheritdoc />
    public async Task<IEnumerable<TodoItemWebApiModel>> GetTasksByTagAsync(int tagId, string userId)
    {
        var uri = new Uri(
            this.httpClient.BaseAddress!,
            $"api/tag/{tagId}/tasks?userId={Uri.EscapeDataString(userId)}");
        var response = await SendAsync(() => this.httpClient.GetAsync(uri));
        _ = response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<IEnumerable<TodoItemWebApiModel>>(JsonOptions);
        return result ?? Enumerable.Empty<TodoItemWebApiModel>();
    }

    private static async Task<HttpResponseMessage> SendAsync(Func<Task<HttpResponseMessage>> send)
    {
        try
        {
            return await send();
        }
        catch (HttpRequestException ex)
        {
            throw new ApiUnavailableException("The Todo List API is not running. Please start the WebApi project.", ex);
        }
        catch (System.Net.Sockets.SocketException ex)
        {
            throw new ApiUnavailableException("The Todo List API is not running. Please start the WebApi project.", ex);
        }
    }
}
