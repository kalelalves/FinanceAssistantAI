using FinIA.Application.Analyses;

namespace FinIA.Tests.Analyses;

public sealed class AnalysisRequestValidatorTests
{
    [Fact]
    public void Validate_ShouldNormalizeAndDeduplicateTickers()
    {
        var validator = new AnalysisRequestValidator();

        var result = validator.Validate(new CreateAnalysisRequest([" petr4 ", "PETR4", "vale3"]));

        Assert.True(result.IsValid);
        Assert.Equal(["PETR4", "VALE3"], result.NormalizedTickers);
    }

    [Fact]
    public void Validate_ShouldRejectMoreThanTenTickers()
    {
        var validator = new AnalysisRequestValidator();
        var tickers = Enumerable.Range(1, 11).Select(index => $"ABCD{index}").ToArray();

        var result = validator.Validate(new CreateAnalysisRequest(tickers));

        Assert.False(result.IsValid);
        Assert.Equal("analysis.too_many_tickers", result.ErrorCode);
    }

    [Fact]
    public void Validate_ShouldRejectInvalidTicker()
    {
        var validator = new AnalysisRequestValidator();

        var result = validator.Validate(new CreateAnalysisRequest(["PETR4F"]));

        Assert.False(result.IsValid);
        Assert.Equal("analysis.invalid_ticker", result.ErrorCode);
    }
}
