namespace FinIA.Application.Analyses;

public interface IAnalysisRequestValidator
{
    AnalysisRequestValidationResult Validate(CreateAnalysisRequest? request);
}
