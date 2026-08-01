using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using FinIA.Application.Health;

namespace FinIA.Functions.Functions;

public sealed class HealthFunction(IHealthService healthService)
{
    [Function("Health")]
    public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "health")] HttpRequest request)
    {
        _ = request;
        return new OkObjectResult(healthService.GetHealth());
    }
}
