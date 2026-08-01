using FinIA.Application.Ai;
using FinIA.Application.Auth;
using FinIA.Application.External.Bcb;
using FinIA.Application.External.Brapi;
using FinIA.Application.Fundamentals;
using FinIA.Application.Persistence;

namespace FinIA.Application.Analyses;

public sealed class AnalysisApplicationService(
    IAnalysisRequestRepository repository,
    IBcbClient bcbClient,
    IBrapiClient brapiClient,
    IFundamentalAnalysisService fundamentalAnalysisService,
    IAiAnalysisService aiAnalysisService) : IAnalysisApplicationService
{
    public async Task<CreateAnalysisResponse> CreateAsync(
        AuthenticatedUser user,
        IReadOnlyCollection<string> normalizedTickers,
        CancellationToken cancellationToken)
    {
        var created = await repository.CreateAsync(
            new CreateAnalysisRecord(user.UserId, normalizedTickers),
            cancellationToken);

        var macro = await bcbClient.GetMacroIndicatorsAsync(cancellationToken);
        var results = new List<AssetAnalysisResponse>();

        foreach (var ticker in created.Tickers)
        {
            var quote = await brapiClient.GetQuoteAsync(ticker, cancellationToken);
            var fundamental = fundamentalAnalysisService.Analyze(new FundamentalAnalysisInput(
                Ticker: ticker,
                CurrentPrice: quote?.RegularMarketPrice,
                DividendYield: quote?.DividendYield,
                PriceToEarnings: quote?.PriceToEarnings,
                SelicAnnual: macro.SelicMeta?.Value,
                Ipca12Months: macro.Ipca12Months?.Value));

            var ai = await aiAnalysisService.AnalyzeAsync(new AiAnalysisRequest(fundamental), cancellationToken);
            results.Add(new AssetAnalysisResponse(
                ai.Ticker,
                ai.CurrentPrice,
                ai.TargetPrice,
                ai.Horizon,
                ai.Diagnosis,
                ai.Summary,
                ai.Source));
        }

        return new CreateAnalysisResponse(
            AnalysisId: created.AnalysisId,
            UserId: created.UserId,
            Status: "completed",
            Results: results);
    }
}
