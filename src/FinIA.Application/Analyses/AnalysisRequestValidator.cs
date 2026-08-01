using System.Text.RegularExpressions;
using FinIA.Domain.Analysis;

namespace FinIA.Application.Analyses;

public sealed partial class AnalysisRequestValidator : IAnalysisRequestValidator
{
    public AnalysisRequestValidationResult Validate(CreateAnalysisRequest? request)
    {
        if (request?.Tickers is null || request.Tickers.Count == 0)
        {
            return AnalysisRequestValidationResult.Failure(
                "analysis.empty_tickers",
                "At least one ticker is required.");
        }

        var normalized = request.Tickers
            .Where(ticker => !string.IsNullOrWhiteSpace(ticker))
            .Select(ticker => ticker.Trim().ToUpperInvariant())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        if (normalized.Length == 0)
        {
            return AnalysisRequestValidationResult.Failure(
                "analysis.empty_tickers",
                "At least one ticker is required.");
        }

        if (normalized.Length > AnalysisLimits.MaxAssetsPerRequest)
        {
            return AnalysisRequestValidationResult.Failure(
                "analysis.too_many_tickers",
                $"A maximum of {AnalysisLimits.MaxAssetsPerRequest} tickers can be analyzed per request.");
        }

        var invalidTicker = normalized.FirstOrDefault(ticker => !TickerPattern().IsMatch(ticker));
        if (invalidTicker is not null)
        {
            return AnalysisRequestValidationResult.Failure(
                "analysis.invalid_ticker",
                $"Ticker '{invalidTicker}' is invalid.");
        }

        return AnalysisRequestValidationResult.Success(normalized);
    }

    [GeneratedRegex("^[A-Z]{4}[0-9]{1,2}$", RegexOptions.CultureInvariant)]
    private static partial Regex TickerPattern();
}
