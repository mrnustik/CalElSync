using System.Collections.Generic;
using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace CalElSync.Function;

public class RunSync
{
    private readonly ILogger _logger;

    public RunSync(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<RunSync>();
    }

    [Function("RunSync_Http")]
    public HttpResponseData RunSync_Http(
        [HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req,
        FunctionContext executionContext)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");

        var response = req.CreateResponse(HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

        response.WriteString("Welcome to Azure Functions!");

        return response;
        
    }
}