using FinIA.Domain.Analysis;

namespace FinIA.Application.Fundamentals;

public sealed class FundamentalAnalysisService : IFundamentalAnalysisService
{
    public FundamentalAnalysisResult Analyze(FundamentalAnalysisInput input)
    {
        var score = 50m;
        var reasons = new List<string>();

        if (input.DividendYield is > 0 && input.SelicAnnual is > 0)
        {
            var annualDividendYield = NormalizeYield(input.DividendYield.Value);
            if (annualDividendYield >= input.SelicAnnual.Value)
            {
                score += 15;
                reasons.Add("Dividend yield acima ou proximo da Selic.");
            }
            else
            {
                score -= 8;
                reasons.Add("Dividend yield abaixo da Selic.");
            }
        }
        else
        {
            score -= 5;
            reasons.Add("Dividend yield ausente ou insuficiente.");
        }

        if (input.PriceToEarnings is > 0)
        {
            if (input.PriceToEarnings <= 8)
            {
                score += 15;
                reasons.Add("P/L indica valuation descontado.");
            }
            else if (input.PriceToEarnings <= 15)
            {
                score += 5;
                reasons.Add("P/L dentro de faixa moderada.");
            }
            else
            {
                score -= 12;
                reasons.Add("P/L elevado exige cautela.");
            }
        }
        else
        {
            score -= 10;
            reasons.Add("P/L ausente ou negativo dificulta avaliacao.");
        }

        if (input.SelicAnnual is > 0 && input.Ipca12Months is > 0)
        {
            var realRate = input.SelicAnnual.Value - input.Ipca12Months.Value;
            if (realRate > 4)
            {
                score -= 5;
                reasons.Add("Juro real alto aumenta exigencia de retorno.");
            }
        }

        score = Math.Clamp(score, 0, 100);
        var diagnosis = GetDiagnosis(score);
        var horizon = GetHorizon(score, input.PriceToEarnings);
        var targetPrice = EstimateTargetPrice(input.CurrentPrice, score);

        return new FundamentalAnalysisResult(
            input.Ticker,
            input.CurrentPrice,
            targetPrice,
            horizon,
            diagnosis,
            score,
            reasons.Take(3).ToArray());
    }

    private static decimal NormalizeYield(decimal dividendYield)
    {
        return dividendYield <= 1 ? dividendYield * 100 : dividendYield;
    }

    private static InvestmentDiagnosis GetDiagnosis(decimal score)
    {
        return score switch
        {
            >= 75 => InvestmentDiagnosis.Buy,
            >= 55 => InvestmentDiagnosis.Hold,
            >= 40 => InvestmentDiagnosis.Watch,
            _ => InvestmentDiagnosis.Avoid
        };
    }

    private static InvestmentHorizon GetHorizon(decimal score, decimal? priceToEarnings)
    {
        if (score >= 70 && priceToEarnings is > 0 and <= 10)
        {
            return InvestmentHorizon.MediumTerm;
        }

        return score >= 55 ? InvestmentHorizon.LongTerm : InvestmentHorizon.ShortTerm;
    }

    private static decimal? EstimateTargetPrice(decimal? currentPrice, decimal score)
    {
        if (currentPrice is null or <= 0)
        {
            return null;
        }

        var upsideFactor = (score - 50) / 100;
        var target = currentPrice.Value * (1 + upsideFactor);
        return decimal.Round(target, 2, MidpointRounding.AwayFromZero);
    }
}
