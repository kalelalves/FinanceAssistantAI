using FinIA.Application.Ai;
using FinIA.Application.Configuration;
using FinIA.Application.Fundamentals;
using FinIA.Domain.Analysis;
using FinIA.Infrastructure.Ai;
using FinIA.Tests.Support;

namespace FinIA.Tests.Ai;

public sealed class OpenAiAnalysisServiceTests
{
    [Fact]
    public async Task AnalyzeAsync_ShouldFallbackWhenApiKeyIsMissing()
    {
        var handler = new StubHttpMessageHandler(_ => throw new InvalidOperationException("Should not call OpenAI."));
        var service = new OpenAiAnalysisService(
            new HttpClient(handler) { BaseAddress = new Uri("https://api.openai.com") },
            new FinIaOptions { OpenAiApiKey = null });

        var result = await service.AnalyzeAsync(
            new AiAnalysisRequest(CreateFundamentalResult()),
            CancellationToken.None);

        Assert.Equal("fallback", result.Source);
    }

    private static FundamentalAnalysisResult CreateFundamentalResult()
    {
        return new FundamentalAnalysisResult(
            Ticker: "PETR4",
            CurrentPrice: 40m,
            TargetPrice: 50m,
            Horizon: InvestmentHorizon.MediumTerm,
            Diagnosis: InvestmentDiagnosis.Buy,
            Score: 80m,
            Reasons: ["P/L descontado."]);
    }
}
