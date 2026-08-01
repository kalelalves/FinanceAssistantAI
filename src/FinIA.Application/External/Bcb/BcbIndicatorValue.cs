namespace FinIA.Application.External.Bcb;

public sealed record BcbIndicatorValue(
    BcbSeriesCode Series,
    DateOnly Date,
    decimal Value);
