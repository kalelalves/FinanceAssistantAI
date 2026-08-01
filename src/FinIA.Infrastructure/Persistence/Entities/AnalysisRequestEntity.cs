namespace FinIA.Infrastructure.Persistence.Entities;

public sealed class AnalysisRequestEntity
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string Status { get; set; } = "pending";

    public int AssetsCount { get; set; }

    public DateTimeOffset RequestedAt { get; set; } = DateTimeOffset.UtcNow;

    public DateTimeOffset? CompletedAt { get; set; }

    public string? ErrorMessage { get; set; }

    public ICollection<AnalysisAssetEntity> Assets { get; set; } = [];
}
