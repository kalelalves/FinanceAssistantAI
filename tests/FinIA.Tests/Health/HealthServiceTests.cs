using FinIA.Application.Configuration;
using FinIA.Application.Health;

namespace FinIA.Tests.Health;

public sealed class HealthServiceTests
{
    [Fact]
    public void GetHealth_ShouldReturnDegradedWhenConfigurationIsIncomplete()
    {
        var service = new HealthService(new ConfigurationValidationResult(false, ["OPENAI_API_KEY"]));

        var result = service.GetHealth();

        Assert.Equal("Degraded", result.Status);
        Assert.Contains("OPENAI_API_KEY", result.MissingSettings);
    }
}
