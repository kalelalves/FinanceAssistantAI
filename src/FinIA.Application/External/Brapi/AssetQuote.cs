namespace FinIA.Application.External.Brapi;

public sealed record AssetQuote(
    string Ticker,
    decimal? RegularMarketPrice,
    decimal? DividendYield,
    decimal? PriceToEarnings,
    string? Currency,
    string? LongName);
