using FinIA.Application.Auth;

namespace FinIA.Application.Analyses;

public interface IAnalysisApplicationService
{
    CreateAnalysisResponse Create(AuthenticatedUser user, IReadOnlyCollection<string> normalizedTickers);
}
