using FinIA.Application.Configuration;

namespace FinIA.Tests.Configuration;

public sealed class FinIaOptionsValidatorTests
{
    [Fact]
    public void Validate_ShouldReportMissingRequiredSettings()
    {
        var result = FinIaOptionsValidator.Validate(new FinIaOptions());

        Assert.False(result.IsValid);
        Assert.Contains(nameof(FinIaOptions.BrapiToken), result.MissingSettings);
        Assert.Contains(nameof(FinIaOptions.OpenAiApiKey), result.MissingSettings);
        Assert.Contains(nameof(FinIaOptions.SupabaseConnectionString), result.MissingSettings);
    }

    [Fact]
    public void Validate_ShouldAcceptCompleteConfiguration()
    {
        var result = FinIaOptionsValidator.Validate(new FinIaOptions
        {
            BrapiToken = "brapi-token",
            OpenAiApiKey = "openai-key",
            SupabaseUrl = "https://example.supabase.co",
            SupabaseJwtSecret = "jwt-secret",
            SupabaseConnectionString = "Host=localhost",
            MaxAssetsPerAnalysis = 10
        });

        Assert.True(result.IsValid);
        Assert.Empty(result.MissingSettings);
    }
}
