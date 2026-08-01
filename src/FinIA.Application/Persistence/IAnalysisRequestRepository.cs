namespace FinIA.Application.Persistence;

public interface IAnalysisRequestRepository
{
    Task<CreatedAnalysisRecord> CreateAsync(CreateAnalysisRecord record, CancellationToken cancellationToken);
}
