using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace AzureFunctionsPGPEncrypt
{
    public class SimonFunction
    {
        private readonly ILogger<SimonFunction> _logger;

        public SimonFunction(ILogger<SimonFunction> logger)
        {
            _logger = logger;
        }

        [Function("SimonFunction")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request to SimonFunction.");
            return new OkObjectResult("Hi ADI!!! Welcome to Azure Functions!");
        }
    }
}
