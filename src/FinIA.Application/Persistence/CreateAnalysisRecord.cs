namespace FinIA.Application.Persistence;

public sealed record CreateAnalysisRecord(
    Guid AnonymizedUserId,
    IReadOnlyCollection<string> Tickers);
