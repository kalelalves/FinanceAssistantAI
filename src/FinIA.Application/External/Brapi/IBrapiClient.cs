namespace FinIA.Application.External.Brapi;

public interface IBrapiClient
{
    Task<AssetQuote?> GetQuoteAsync(string ticker, CancellationToken cancellationToken);
}
