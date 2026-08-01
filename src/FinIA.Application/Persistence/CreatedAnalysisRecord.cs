namespace FinIA.Application.Persistence;

public sealed record CreatedAnalysisRecord(
    Guid AnalysisId,
    Guid AnonymizedUserId,
    string Status,
    IReadOnlyCollection<string> Tickers);
