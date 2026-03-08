using System.Text.Json.Serialization;

namespace CalElSync.Tasks.Todoist.Client.Requests;

public record CreateTodoistTaskRequest(
    string Content,
    string ProjectId,
    DateTime DueDatetime,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] int? Duration,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] string? DurationUnit
);
