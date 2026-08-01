using FinIA.Application.Fundamentals;

namespace FinIA.Application.Ai;

public static class AiFallbackFormatter
{
    public static AiAnalysisResponse Format(FundamentalAnalysisResult result)
    {
        var summary = result.Reasons.Count == 0
            ? "Dados insuficientes para justificativa detalhada."
            : string.Join(" ", result.Reasons.Take(3));

        return new AiAnalysisResponse(
            result.Ticker,
            result.CurrentPrice,
            result.TargetPrice,
            result.Horizon,
            result.Diagnosis,
            summary,
            Source: "fallback");
    }
}
