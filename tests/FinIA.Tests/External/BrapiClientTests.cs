using FinIA.Application.Configuration;
using FinIA.Infrastructure.External.Brapi;
using FinIA.Tests.Support;
using System.Net;

namespace FinIA.Tests.External;

public sealed class BrapiClientTests
{
    [Fact]
    public async Task GetQuoteAsync_ShouldParseQuoteAndUseToken()
    {
        var handler = new StubHttpMessageHandler(_ => StubHttpMessageHandler.Json("""
            {"results":[{"symbol":"PETR4","regularMarketPrice":38.42,"dividendYield":0.12,"priceToEarnings":5.6,"currency":"BRL","longName":"Petrobras PN"}]}
            """));
        var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://brapi.dev") };
        var client = new BrapiClient(httpClient, new FinIaOptions { BrapiToken = "token-123" });

        var result = await client.GetQuoteAsync("PETR4", CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("PETR4", result.Ticker);
        Assert.Equal(38.42m, result.RegularMarketPrice);
        Assert.Contains("token=token-123", handler.LastRequest?.RequestUri?.Query);
    }

    [Fact]
    public async Task GetQuoteAsync_ShouldReturnNullWhenBrapiRejectsRequest()
    {
        var handler = new StubHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.Unauthorized));
        var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://brapi.dev") };
        var client = new BrapiClient(httpClient, new FinIaOptions { BrapiToken = "invalid-token" });

        var result = await client.GetQuoteAsync("PETR4", CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetQuoteAsync_ShouldReturnNullWhenBrapiTimesOut()
    {
        var handler = new StubHttpMessageHandler(_ => throw new TaskCanceledException("timeout"));
        var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://brapi.dev") };
        var client = new BrapiClient(httpClient, new FinIaOptions());

        var result = await client.GetQuoteAsync("PETR4", CancellationToken.None);

        Assert.Null(result);
    }
}
