using FinIA.Domain.Analysis;

namespace FinIA.Application.Configuration;

public static class FinIaOptionsValidator
{
    public static ConfigurationValidationResult Validate(FinIaOptions options)
    {
        var missing = new List<string>();

        AddIfMissing(missing, nameof(options.BrapiToken), options.BrapiToken);
        AddIfMissing(missing, nameof(options.OpenAiApiKey), options.OpenAiApiKey);
        AddIfMissing(missing, nameof(options.SupabaseUrl), options.SupabaseUrl);
        AddIfMissing(missing, nameof(options.SupabaseJwtSecret), options.SupabaseJwtSecret);
        AddIfMissing(missing, nameof(options.SupabaseConnectionString), options.SupabaseConnectionString);

        if (options.MaxAssetsPerAnalysis != AnalysisLimits.MaxAssetsPerRequest)
        {
            missing.Add($"{nameof(options.MaxAssetsPerAnalysis)} must be {AnalysisLimits.MaxAssetsPerRequest}");
        }

        return missing.Count == 0
            ? ConfigurationValidationResult.Valid
            : new ConfigurationValidationResult(false, missing);
    }

    private static void AddIfMissing(ICollection<string> missing, string name, string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            missing.Add(name);
        }
    }
}
