using FinIA.Domain.Analysis;

namespace FinIA.Tests;

public sealed class AnalysisLimitsTests
{
    [Fact]
    public void MaxAssetsPerRequest_ShouldBeTen()
    {
        Assert.Equal(10, AnalysisLimits.MaxAssetsPerRequest);
    }
}
