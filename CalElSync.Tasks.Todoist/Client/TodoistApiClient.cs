using System.Net.Http.Json;
using System.Text.Json;
using CalElSync.Tasks.Todoist.Client.Requests;
using CalElSync.Tasks.Todoist.Client.Responses;

namespace CalElSync.Tasks.Todoist.Client;

public class TodoistApiClient : ITodoistApiClient
{
    private readonly HttpClient _httpClient;

    private readonly JsonSerializerOptions _serializationOptions = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
    };

    public TodoistApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IReadOnlyCollection<TodoistTaskResponse>> GetTasksAsync(
        TasksFilter filter,
        CancellationToken ct
    )
    {
        var request = new HttpRequestMessage(
            HttpMethod.Get,
            $"/api/v1/tasks?project_id={filter.ProjectId}"
        );
        var response = await _httpClient.SendAsync(request, ct);
        response.EnsureSuccessStatusCode();
        var stringResponse = await response.Content.ReadAsStringAsync(ct);
        var wrapper = JsonSerializer.Deserialize<TodoistPagedResponse<TodoistTaskResponse>>(
            stringResponse,
            _serializationOptions
        )!;
        return wrapper.Results;
    }

    public async Task CreateTaskAsync(
        CreateTodoistTaskRequest creationRequest,
        CancellationToken ct
    )
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/tasks");
        request.Content = JsonContent.Create(creationRequest, options: _serializationOptions);
        var response = await _httpClient.SendAsync(request, ct);
        response.EnsureSuccessStatusCode();
    }
}
