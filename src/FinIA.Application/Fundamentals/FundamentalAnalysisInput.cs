namespace FinIA.Application.Fundamentals;

public sealed record FundamentalAnalysisInput(
    string Ticker,
    decimal? CurrentPrice,
    decimal? DividendYield,
    decimal? PriceToEarnings,
    decimal? SelicAnnual,
    decimal? Ipca12Months);
