namespace FinIA.Application.External.Bcb;

public interface IBcbClient
{
    Task<BcbIndicatorValue?> GetLatestAsync(BcbSeriesCode series, CancellationToken cancellationToken);

    Task<MacroIndicators> GetMacroIndicatorsAsync(CancellationToken cancellationToken);
}
