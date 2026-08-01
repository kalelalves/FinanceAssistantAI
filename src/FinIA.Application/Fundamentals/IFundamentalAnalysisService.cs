namespace FinIA.Application.Fundamentals;

public interface IFundamentalAnalysisService
{
    FundamentalAnalysisResult Analyze(FundamentalAnalysisInput input);
}
