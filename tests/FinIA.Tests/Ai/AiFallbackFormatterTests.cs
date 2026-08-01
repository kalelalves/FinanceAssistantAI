using FinIA.Application.Ai;
using FinIA.Application.Fundamentals;
using FinIA.Domain.Analysis;

namespace FinIA.Tests.Ai;

public sealed class AiFallbackFormatterTests
{
    [Fact]
    public void Format_ShouldPreserveBackendNumbers()
    {
        var response = AiFallbackFormatter.Format(new FundamentalAnalysisResult(
            Ticker: "PETR4",
            CurrentPrice: 40m,
            TargetPrice: 50m,
            Horizon: InvestmentHorizon.MediumTerm,
            Diagnosis: InvestmentDiagnosis.Buy,
            Score: 80m,
            Reasons: ["P/L descontado.", "Dividend yield acima da Selic."]));

        Assert.Equal("PETR4", response.Ticker);
        Assert.Equal(50m, response.TargetPrice);
        Assert.Equal(InvestmentDiagnosis.Buy, response.Diagnosis);
        Assert.Equal("fallback", response.Source);
    }
}
