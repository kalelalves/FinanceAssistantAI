namespace FinIA.Application.Analyses;

public sealed record AnalysisRequestValidationResult(
    bool IsValid,
    IReadOnlyCollection<string> NormalizedTickers,
    string? ErrorCode,
    string? ErrorMessage)
{
    public static AnalysisRequestValidationResult Success(IReadOnlyCollection<string> normalizedTickers)
        => new(true, normalizedTickers, null, null);

    public static AnalysisRequestValidationResult Failure(string code, string message)
        => new(false, [], code, message);
}
