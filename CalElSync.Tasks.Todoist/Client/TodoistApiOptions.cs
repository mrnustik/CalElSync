using System.ComponentModel.DataAnnotations;

namespace CalElSync.Tasks.Todoist.Client;

public class TodoistApiOptions
{
    [Required] public required string ApiKey { get; init; }
}