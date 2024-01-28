using System.Net;
using CalElSync.Core;
using CalElSync.Core.Common;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace CalElSync.Function;

public class RunSync
{
    private readonly ISynchronizationHandler _synchronizationHandler;
    private readonly ILogger<RunSync> _logger;

    public RunSync(ISynchronizationHandler synchronizationHandler, ILoggerFactory loggerFactory)
    {
        _synchronizationHandler = synchronizationHandler;
        _logger = loggerFactory.CreateLogger<RunSync>();
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

    [Function("RunSync_Weekly")]
    public async Task RunAsync(
        [TimerTrigger("0 2 * * 1")] TimerInfo timerInfo,
        FunctionContext executionContext)
    {
        _logger.LogInformation($"Import function executed at: {DateTime.UtcNow}");
        await _synchronizationHandler.RunSynchronizationAsync(
            new DateTimeInterval(
                DateTime.Today,
                DateTime.Today.AddDays(7)),
            executionContext.CancellationToken);
    }
}