using FinIA.Application.Persistence;
using FinIA.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FinIA.Tests.Persistence;

public sealed class EfAnalysisRequestRepositoryTests
{
    [Fact]
    public async Task CreateAsync_ShouldPersistRequestAndAssets()
    {
        await using var dbContext = CreateDbContext();
        var repository = new EfAnalysisRequestRepository(dbContext);
        var anonymizedUserId = Guid.NewGuid();

        var created = await repository.CreateAsync(
            new CreateAnalysisRecord(anonymizedUserId, ["PETR4", "VALE3"]),
            CancellationToken.None);

        Assert.Equal(anonymizedUserId, created.AnonymizedUserId);
        Assert.Equal("pending", created.Status);
        Assert.Equal(["PETR4", "VALE3"], created.Tickers);
        Assert.Equal(1, await dbContext.AnalysisRequests.CountAsync());
        Assert.Equal(anonymizedUserId, await dbContext.AnalysisRequests.Select(x => x.AnonymizedUserId).SingleAsync());
        Assert.Equal(2, await dbContext.AnalysisAssets.CountAsync());
    }

    private static FinIaDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<FinIaDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new FinIaDbContext(options);
    }
}
