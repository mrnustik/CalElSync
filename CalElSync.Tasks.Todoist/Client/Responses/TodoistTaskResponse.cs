namespace CalElSync.Tasks.Todoist.Client.Responses;

public record TodoistTaskResponse(
    string ProjectId,
    string Content,
    TodoistDateResponse? Due
);