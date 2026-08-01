namespace FinIA.Application.Persistence;

public sealed record CreateAnalysisRecord(
    Guid UserId,
    IReadOnlyCollection<string> Tickers);
