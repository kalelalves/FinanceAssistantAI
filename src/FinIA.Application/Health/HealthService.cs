using FinIA.Application.Configuration;

namespace FinIA.Application.Health;

public sealed class HealthService(ConfigurationValidationResult configurationValidation) : IHealthService
{
    public HealthResponse GetHealth()
    {
        var status = configurationValidation.IsValid ? "Healthy" : "Degraded";

        return new HealthResponse(
            Status: status,
            Service: "FinIA Functions",
            Runtime: ".NET 10 Azure Functions Isolated Worker",
            CheckedAtUtc: DateTimeOffset.UtcNow,
            MissingSettings: configurationValidation.MissingSettings);
    }
}
