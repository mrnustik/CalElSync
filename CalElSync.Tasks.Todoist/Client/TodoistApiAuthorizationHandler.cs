using System.Net.Http.Headers;
using Microsoft.Extensions.Options;

namespace CalElSync.Tasks.Todoist.Client;

public class TodoistApiAuthorizationHandler : DelegatingHandler
{
    private readonly IOptions<TodoistApiOptions> _options;

    public TodoistApiAuthorizationHandler(IOptions<TodoistApiOptions> options)
    {
        _options = options;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _options.Value.ApiKey);
        return base.SendAsync(request, cancellationToken);
    }
}