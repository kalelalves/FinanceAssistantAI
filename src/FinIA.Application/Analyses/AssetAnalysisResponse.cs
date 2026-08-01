using FinIA.Domain.Analysis;

namespace FinIA.Application.Analyses;

public sealed record AssetAnalysisResponse(
    string Ticker,
    decimal? CurrentPrice,
    decimal? TargetPrice,
    InvestmentHorizon Horizon,
    InvestmentDiagnosis Diagnosis,
    string Summary,
    string Source);
