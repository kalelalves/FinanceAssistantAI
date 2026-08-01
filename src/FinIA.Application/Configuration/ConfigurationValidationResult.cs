namespace FinIA.Application.Configuration;

public sealed record ConfigurationValidationResult(
    bool IsValid,
    IReadOnlyCollection<string> MissingSettings)
{
    public static ConfigurationValidationResult Valid { get; } = new(true, []);
}
