using FinIA.Application.Auth;
using FinIA.Application.Persistence;

namespace FinIA.Application.Analyses;

public sealed class AnalysisApplicationService(IAnalysisRequestRepository repository) : IAnalysisApplicationService
{
    public async Task<CreateAnalysisResponse> CreateAsync(
        AuthenticatedUser user,
        IReadOnlyCollection<string> normalizedTickers,
        CancellationToken cancellationToken)
    {
        var created = await repository.CreateAsync(
            new CreateAnalysisRecord(user.UserId, normalizedTickers),
            cancellationToken);

        return new CreateAnalysisResponse(
            AnalysisId: created.AnalysisId,
            UserId: created.UserId,
            Status: created.Status,
            Tickers: created.Tickers);
    }
}
