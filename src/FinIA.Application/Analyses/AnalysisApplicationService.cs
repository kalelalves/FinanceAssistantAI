using FinIA.Application.Auth;

namespace FinIA.Application.Analyses;

public sealed class AnalysisApplicationService : IAnalysisApplicationService
{
    public CreateAnalysisResponse Create(AuthenticatedUser user, IReadOnlyCollection<string> normalizedTickers)
    {
        return new CreateAnalysisResponse(
            AnalysisId: Guid.NewGuid(),
            UserId: user.UserId,
            Status: "accepted",
            Tickers: normalizedTickers);
    }
}
