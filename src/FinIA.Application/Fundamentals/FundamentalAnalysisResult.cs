using FinIA.Domain.Analysis;

namespace FinIA.Application.Fundamentals;

public sealed record FundamentalAnalysisResult(
    string Ticker,
    decimal? CurrentPrice,
    decimal? TargetPrice,
    InvestmentHorizon Horizon,
    InvestmentDiagnosis Diagnosis,
    decimal Score,
    IReadOnlyCollection<string> Reasons);
