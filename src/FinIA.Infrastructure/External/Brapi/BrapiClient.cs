using System.Net.Http.Json;
using System.Text.Json.Serialization;
using FinIA.Application.Configuration;
using FinIA.Application.External.Brapi;

namespace FinIA.Infrastructure.External.Brapi;

public sealed class BrapiClient(HttpClient httpClient, FinIaOptions options) : IBrapiClient
{
    public async Task<AssetQuote?> GetQuoteAsync(string ticker, CancellationToken cancellationToken)
    {
        var path = $"/api/quote/{Uri.EscapeDataString(ticker)}?fundamental=true";
        if (!string.IsNullOrWhiteSpace(options.BrapiToken))
        {
            path += $"&token={Uri.EscapeDataString(options.BrapiToken)}";
        }

        HttpResponseMessage httpResponse;
        try
        {
            httpResponse = await httpClient.GetAsync(path, cancellationToken);
        }
        catch (HttpRequestException)
        {
            return null;
        }
        catch (TaskCanceledException)
        {
            return null;
        }

        using (httpResponse)
        {
            if (!httpResponse.IsSuccessStatusCode)
            {
                return null;
            }

            var response = await httpResponse.Content.ReadFromJsonAsync<BrapiQuoteResponse>(cancellationToken);
            var result = response?.Results?.FirstOrDefault();

            return result is null
                ? null
                : new AssetQuote(
                    Ticker: result.Symbol ?? ticker,
                    RegularMarketPrice: result.RegularMarketPrice,
                    DividendYield: result.DividendYield,
                    PriceToEarnings: result.PriceToEarnings,
                    Currency: result.Currency,
                    LongName: result.LongName);
        }
    }

    public static void Configure(HttpClient client, FinIaOptions options)
    {
        client.BaseAddress = new Uri(options.BrapiBaseUrl);
        client.Timeout = TimeSpan.FromSeconds(10);
    }

    private sealed record BrapiQuoteResponse(
        [property: JsonPropertyName("results")] IReadOnlyCollection<BrapiQuoteResult>? Results);

    private sealed record BrapiQuoteResult(
        [property: JsonPropertyName("symbol")] string? Symbol,
        [property: JsonPropertyName("regularMarketPrice")] decimal? RegularMarketPrice,
        [property: JsonPropertyName("dividendYield")] decimal? DividendYield,
        [property: JsonPropertyName("priceToEarnings")] decimal? PriceToEarnings,
        [property: JsonPropertyName("currency")] string? Currency,
        [property: JsonPropertyName("longName")] string? LongName);
}
