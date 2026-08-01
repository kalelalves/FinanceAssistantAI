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
        Assert.Contains(nameof(FinIaOptions.AnonymizationSecret), result.MissingSettings);
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
            AnonymizationSecret = "anonymization-secret-with-enough-entropy",
            MaxAssetsPerAnalysis = 10
        });

        Assert.True(result.IsValid);
        Assert.Empty(result.MissingSettings);
    }

    [Fact]
    public void Validate_ShouldReportWeakAnonymizationSecret()
    {
        var result = FinIaOptionsValidator.Validate(new FinIaOptions
        {
            BrapiToken = "brapi-token",
            OpenAiApiKey = "openai-key",
            SupabaseUrl = "https://example.supabase.co",
            SupabaseJwtSecret = "jwt-secret",
            SupabaseConnectionString = "Host=localhost",
            AnonymizationSecret = "short",
            MaxAssetsPerAnalysis = 10
        });

        Assert.False(result.IsValid);
        Assert.Contains("AnonymizationSecret must be at least 32 characters", result.MissingSettings);
    }
}
