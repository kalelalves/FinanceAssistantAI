using FinIA.Application.Persistence;
using FinIA.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace FinIA.Infrastructure.Persistence;

public sealed class EfAnalysisRequestRepository(FinIaDbContext dbContext) : IAnalysisRequestRepository
{
    public async Task<CreatedAnalysisRecord> CreateAsync(CreateAnalysisRecord record, CancellationToken cancellationToken)
    {
        var request = new AnalysisRequestEntity
        {
            Id = Guid.NewGuid(),
            AnonymizedUserId = record.AnonymizedUserId,
            Status = "pending",
            AssetsCount = record.Tickers.Count,
            RequestedAt = DateTimeOffset.UtcNow,
            Assets = record.Tickers.Select(ticker => new AnalysisAssetEntity
            {
                Id = Guid.NewGuid(),
                Ticker = ticker,
                CreatedAt = DateTimeOffset.UtcNow
            }).ToArray()
        };

        dbContext.AnalysisRequests.Add(request);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new CreatedAnalysisRecord(
            request.Id,
            request.AnonymizedUserId,
            request.Status,
            request.Assets.Select(asset => asset.Ticker).ToArray());
    }
}
