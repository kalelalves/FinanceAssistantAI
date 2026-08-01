namespace FinIA.Infrastructure.Persistence.Entities;

public sealed class MacroIndicatorsSnapshotEntity
{
    public Guid Id { get; set; }

    public Guid AnalysisRequestId { get; set; }

    public decimal? SelicMetaAnnual { get; set; }

    public decimal? IpcaMonthly { get; set; }

    public decimal? Ipca12M { get; set; }

    public decimal? UsdPtaxSell { get; set; }

    public decimal? SavingsMonthly { get; set; }

    public string Source { get; set; } = "bcb_sgs";

    public string? RawPayload { get; set; }

    public DateTimeOffset CapturedAt { get; set; } = DateTimeOffset.UtcNow;
}
