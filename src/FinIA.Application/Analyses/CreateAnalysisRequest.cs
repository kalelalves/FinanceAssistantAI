namespace FinIA.Application.Analyses;

public sealed record CreateAnalysisRequest(
    IReadOnlyCollection<string>? Tickers);
