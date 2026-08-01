namespace FinIA.Application.Persistence;

public sealed record CreatedAnalysisRecord(
    Guid AnalysisId,
    Guid UserId,
    string Status,
    IReadOnlyCollection<string> Tickers);
