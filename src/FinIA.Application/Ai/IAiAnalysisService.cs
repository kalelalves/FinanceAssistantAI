namespace FinIA.Application.Ai;

public interface IAiAnalysisService
{
    Task<AiAnalysisResponse> AnalyzeAsync(AiAnalysisRequest request, CancellationToken cancellationToken);
}
