namespace CalElSync.Tasks.Todoist.Client.Responses;

public record TodoistPagedResponse<T>(IReadOnlyCollection<T> Results, string? NextCursor);
