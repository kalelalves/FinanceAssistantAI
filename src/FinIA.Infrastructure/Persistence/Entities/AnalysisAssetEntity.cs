namespace FinIA.Infrastructure.Persistence.Entities;

public sealed class AnalysisAssetEntity
{
    public Guid Id { get; set; }

    public Guid AnalysisRequestId { get; set; }

    public string Ticker { get; set; } = string.Empty;

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public AnalysisRequestEntity? AnalysisRequest { get; set; }
}
