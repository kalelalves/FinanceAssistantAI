using FinIA.Domain.Analysis;

namespace FinIA.Application.Ai;

public sealed record AiAnalysisResponse(
    string Ticker,
    decimal? CurrentPrice,
    decimal? TargetPrice,
    InvestmentHorizon Horizon,
    InvestmentDiagnosis Diagnosis,
    string Summary,
    string Source);
