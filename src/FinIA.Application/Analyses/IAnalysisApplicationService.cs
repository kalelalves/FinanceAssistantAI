using FinIA.Application.Auth;

namespace FinIA.Application.Analyses;

public interface IAnalysisApplicationService
{
    Task<CreateAnalysisResponse> CreateAsync(
        AuthenticatedUser user,
        IReadOnlyCollection<string> normalizedTickers,
        CancellationToken cancellationToken);
}
