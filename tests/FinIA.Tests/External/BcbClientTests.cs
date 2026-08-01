using FinIA.Application.External.Bcb;
using FinIA.Infrastructure.External.Bcb;
using FinIA.Tests.Support;

namespace FinIA.Tests.External;

public sealed class BcbClientTests
{
    [Fact]
    public async Task GetLatestAsync_ShouldParseBcbResponse()
    {
        var handler = new StubHttpMessageHandler(_ => StubHttpMessageHandler.Json("""
            [{"data":"01/08/2026","valor":"15.00"}]
            """));
        var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.bcb.gov.br") };
        var client = new BcbClient(httpClient);

        var result = await client.GetLatestAsync(BcbSeriesCode.SelicMeta, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(BcbSeriesCode.SelicMeta, result.Series);
        Assert.Equal(new DateOnly(2026, 8, 1), result.Date);
        Assert.Equal(15.00m, result.Value);
    }
}
