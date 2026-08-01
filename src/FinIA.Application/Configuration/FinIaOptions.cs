namespace FinIA.Application.Configuration;

public sealed class FinIaOptions
{
    public const string SectionName = "FinIA";

    public string BcbBaseUrl { get; init; } = "https://api.bcb.gov.br";

    public string BrapiBaseUrl { get; init; } = "https://brapi.dev";

    public string? BrapiToken { get; init; }

    public string? OpenAiApiKey { get; init; }

    public string OpenAiModel { get; init; } = "gpt-4o-mini";

    public string? SupabaseUrl { get; init; }

    public string? SupabaseJwtSecret { get; init; }

    public string? SupabaseConnectionString { get; init; }

    public string? AnonymizationSecret { get; init; }

    public int MaxAssetsPerAnalysis { get; init; } = 10;
}
