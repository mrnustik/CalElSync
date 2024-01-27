using System.ComponentModel.DataAnnotations;

namespace CalElSync.Tasks.Todoist.Client;

public class TodoistApiOptions
{
    [Required]
    public string ApiKey { get; set; }
}