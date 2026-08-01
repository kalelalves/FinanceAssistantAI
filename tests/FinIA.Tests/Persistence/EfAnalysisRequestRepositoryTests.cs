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
        var userId = Guid.NewGuid();

        var created = await repository.CreateAsync(
            new CreateAnalysisRecord(userId, ["PETR4", "VALE3"]),
            CancellationToken.None);

        Assert.Equal(userId, created.UserId);
        Assert.Equal("pending", created.Status);
        Assert.Equal(["PETR4", "VALE3"], created.Tickers);
        Assert.Equal(1, await dbContext.AnalysisRequests.CountAsync());
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
