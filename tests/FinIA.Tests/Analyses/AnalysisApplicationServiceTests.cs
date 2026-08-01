using FinIA.Application.Ai;
using FinIA.Application.Analyses;
using FinIA.Application.Auth;
using FinIA.Application.External.Bcb;
using FinIA.Application.External.Brapi;
using FinIA.Application.Fundamentals;
using FinIA.Application.Persistence;

namespace FinIA.Tests.Analyses;

public sealed class AnalysisApplicationServiceTests
{
    [Fact]
    public async Task CreateAsync_ShouldReturnCompletedAnalysisWithResults()
    {
        var service = new AnalysisApplicationService(
            new FakeRepository(),
            new FakeBcbClient(),
            new FakeBrapiClient(),
            new FundamentalAnalysisService(),
            new FakeAiService());

        var response = await service.CreateAsync(
            new AuthenticatedUser(Guid.NewGuid(), "user@example.com"),
            ["PETR4"],
            CancellationToken.None);

        Assert.Equal("completed", response.Status);
        Assert.Single(response.Results);
        Assert.Equal("PETR4", response.Results.First().Ticker);
    }

    private sealed class FakeRepository : IAnalysisRequestRepository
    {
        public Task<CreatedAnalysisRecord> CreateAsync(CreateAnalysisRecord record, CancellationToken cancellationToken)
        {
            return Task.FromResult(new CreatedAnalysisRecord(Guid.NewGuid(), record.UserId, "pending", record.Tickers));
        }
    }

    private sealed class FakeBcbClient : IBcbClient
    {
        public Task<BcbIndicatorValue?> GetLatestAsync(BcbSeriesCode series, CancellationToken cancellationToken)
        {
            return Task.FromResult<BcbIndicatorValue?>(new BcbIndicatorValue(series, new DateOnly(2026, 8, 1), 10m));
        }

        public Task<MacroIndicators> GetMacroIndicatorsAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(new MacroIndicators(
                new BcbIndicatorValue(BcbSeriesCode.SelicMeta, new DateOnly(2026, 8, 1), 10m),
                null,
                new BcbIndicatorValue(BcbSeriesCode.Ipca12Months, new DateOnly(2026, 8, 1), 4m),
                null,
                null));
        }
    }

    private sealed class FakeBrapiClient : IBrapiClient
    {
        public Task<AssetQuote?> GetQuoteAsync(string ticker, CancellationToken cancellationToken)
        {
            return Task.FromResult<AssetQuote?>(new AssetQuote(ticker, 40m, 0.16m, 6m, "BRL", "Teste"));
        }
    }

    private sealed class FakeAiService : IAiAnalysisService
    {
        public Task<AiAnalysisResponse> AnalyzeAsync(AiAnalysisRequest request, CancellationToken cancellationToken)
        {
            return Task.FromResult(AiFallbackFormatter.Format(request.FundamentalResult));
        }
    }
}
