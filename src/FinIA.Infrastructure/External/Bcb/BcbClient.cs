using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using FinIA.Application.Configuration;
using FinIA.Application.External.Bcb;

namespace FinIA.Infrastructure.External.Bcb;

public sealed class BcbClient(HttpClient httpClient) : IBcbClient
{
    public async Task<BcbIndicatorValue?> GetLatestAsync(BcbSeriesCode series, CancellationToken cancellationToken)
    {
        var path = $"/dados/serie/bcdata.sgs.{(int)series}/dados/ultimos/1?formato=json";
        var response = await httpClient.GetFromJsonAsync<BcbSeriesResponse[]>(path, cancellationToken);
        var latest = response?.FirstOrDefault();

        if (latest is null ||
            !DateOnly.TryParseExact(latest.Date, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date) ||
            !decimal.TryParse(latest.Value, NumberStyles.Number, CultureInfo.InvariantCulture, out var value))
        {
            return null;
        }

        return new BcbIndicatorValue(series, date, value);
    }

    public async Task<MacroIndicators> GetMacroIndicatorsAsync(CancellationToken cancellationToken)
    {
        var selic = await GetLatestAsync(BcbSeriesCode.SelicMeta, cancellationToken);
        var ipcaMonthly = await GetLatestAsync(BcbSeriesCode.IpcaMonthly, cancellationToken);
        var ipca12Months = await GetLatestAsync(BcbSeriesCode.Ipca12Months, cancellationToken);
        var usdPtax = await GetLatestAsync(BcbSeriesCode.UsdPtaxSell, cancellationToken);
        var savings = await GetLatestAsync(BcbSeriesCode.SavingsMonthly, cancellationToken);

        return new MacroIndicators(selic, ipcaMonthly, ipca12Months, usdPtax, savings);
    }

    public static void Configure(HttpClient client, FinIaOptions options)
    {
        client.BaseAddress = new Uri(options.BcbBaseUrl);
        client.Timeout = TimeSpan.FromSeconds(10);
    }

    private sealed record BcbSeriesResponse(
        [property: JsonPropertyName("data")] string Date,
        [property: JsonPropertyName("valor")] string Value);
}
