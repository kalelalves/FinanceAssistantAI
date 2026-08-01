namespace FinIA.Application.Analyses;

public sealed record CreateAnalysisResponse(
    Guid AnalysisId,
    Guid UserId,
    string Status,
    IReadOnlyCollection<string> Tickers);
