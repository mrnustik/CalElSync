namespace CalElSync.Tasks.Todoist.Client.Requests;

public record CreateTodoistTaskRequest(
    string Content,
    string ProjectId,
    DateTime DueDatetime);