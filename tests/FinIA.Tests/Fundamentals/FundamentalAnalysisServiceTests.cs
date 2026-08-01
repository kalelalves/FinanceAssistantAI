using FinIA.Application.Fundamentals;
using FinIA.Domain.Analysis;

namespace FinIA.Tests.Fundamentals;

public sealed class FundamentalAnalysisServiceTests
{
    [Fact]
    public void Analyze_ShouldPreferDiscountedAssetWithStrongYield()
    {
        var service = new FundamentalAnalysisService();

        var result = service.Analyze(new FundamentalAnalysisInput(
            Ticker: "PETR4",
            CurrentPrice: 40m,
            DividendYield: 0.16m,
            PriceToEarnings: 6m,
            SelicAnnual: 10m,
            Ipca12Months: 4m));

        Assert.Equal(InvestmentDiagnosis.Buy, result.Diagnosis);
        Assert.True(result.TargetPrice > 40m);
        Assert.True(result.Score >= 75);
    }

    [Fact]
    public void Analyze_ShouldAvoidAssetWithMissingFundamentals()
    {
        var service = new FundamentalAnalysisService();

        var result = service.Analyze(new FundamentalAnalysisInput(
            Ticker: "ABCD3",
            CurrentPrice: 20m,
            DividendYield: null,
            PriceToEarnings: null,
            SelicAnnual: 12m,
            Ipca12Months: 6m));

        Assert.Equal(InvestmentDiagnosis.Avoid, result.Diagnosis);
        Assert.True(result.Reasons.Count > 0);
    }
}
