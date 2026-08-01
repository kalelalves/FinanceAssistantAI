namespace FinIA.Infrastructure.Persistence.Entities;

public sealed class AssetFundamentalsSnapshotEntity
{
    public Guid Id { get; set; }

    public Guid AnalysisAssetId { get; set; }

    public decimal? RegularMarketPrice { get; set; }

    public decimal? DividendYield { get; set; }

    public decimal? PriceToEarnings { get; set; }

    public string Source { get; set; } = "brapi.dev";

    public string? RawPayload { get; set; }

    public DateTimeOffset CapturedAt { get; set; } = DateTimeOffset.UtcNow;
}
