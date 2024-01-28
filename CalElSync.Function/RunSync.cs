using System.Net;
using CalElSync.Core;
using CalElSync.Core.Common;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace CalElSync.Function;

public class RunSync
{
    private readonly ISynchronizationHandler _synchronizationHandler;

    public RunSync(ISynchronizationHandler synchronizationHandler)
    {
        _synchronizationHandler = synchronizationHandler;
    }

    [Function("RunSync_Http")]
    public async Task<HttpResponseData> RunSync_Http(
        [HttpTrigger(AuthorizationLevel.Function, "post")]
        HttpRequestData req,
        FunctionContext executionContext)
    {
        await _synchronizationHandler.RunSynchronizationAsync(
            new DateTimeInterval(
                DateTime.Today,
                DateTime.Today.AddDays(7)),
            executionContext.CancellationToken);
        var response = req.CreateResponse(HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "text/plain; charset=utf-8");
        await response.WriteStringAsync("OK!");
        return response;
    }
}