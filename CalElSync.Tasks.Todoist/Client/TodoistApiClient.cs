using System.Net.Http.Json;
using System.Text.Json;
using CalElSync.Tasks.Todoist.Client.Requests;
using CalElSync.Tasks.Todoist.Client.Responses;
using Microsoft.Extensions.Options;

namespace CalElSync.Tasks.Todoist.Client;

public class TodoistApiClient : ITodoistApiClient
{
    private readonly IOptions<TodoistApiOptions> _options;
    private readonly HttpClient _httpClient;

    private readonly JsonSerializerOptions _serializationOptions = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
    };

    public TodoistApiClient(IOptions<TodoistApiOptions> options, HttpClient httpClient)
    {
        _options = options;
        _httpClient = httpClient;
    }

    public async Task<IReadOnlyCollection<TodoistTaskResponse>> GetTasksAsync(TasksFilter filter, CancellationToken ct)
    {
        var request = new HttpRequestMessage(
            HttpMethod.Get,
            $"https://api.todoist.com/rest/v2/tasks?project_id={filter.ProjectId}");
        request.Headers.Add("Authorization", $"Bearer {_options.Value.ApiKey}");
        var response = await _httpClient.SendAsync(request, ct);
        response.EnsureSuccessStatusCode();
        var stringResponse = await response.Content.ReadAsStringAsync(ct);
        return JsonSerializer.Deserialize<TodoistTaskResponse[]>(stringResponse, _serializationOptions);
    }

    public async Task CreateTaskAsync(CreateTodoistTaskRequest creationRequest, CancellationToken ct)
    {
        var request = new HttpRequestMessage(
            HttpMethod.Post,
            $"https://api.todoist.com/rest/v2/tasks");
        request.Content = JsonContent.Create(creationRequest, options: _serializationOptions);
        request.Headers.Add("Authorization", $"Bearer {_options.Value.ApiKey}");
        var response = await _httpClient.SendAsync(request, ct);
        response.EnsureSuccessStatusCode();
    }
}