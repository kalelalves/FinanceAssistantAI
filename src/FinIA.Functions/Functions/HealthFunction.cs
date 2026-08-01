using FinIA.Application.Health;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;

namespace FinIA.Functions.Functions;

public sealed class HealthFunction
{
    [Function("Health")]
    public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "health")] HttpRequest request)
    {
        var response = new HealthResponse(
            Status: "Healthy",
            Service: "FinIA Functions",
            Runtime: ".NET 10 Azure Functions Isolated Worker",
            CheckedAtUtc: DateTimeOffset.UtcNow);

        return new OkObjectResult(response);
    }
}
