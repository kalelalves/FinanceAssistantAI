namespace FinIA.Application.External.Bcb;

public sealed record MacroIndicators(
    BcbIndicatorValue? SelicMeta,
    BcbIndicatorValue? IpcaMonthly,
    BcbIndicatorValue? Ipca12Months,
    BcbIndicatorValue? UsdPtaxSell,
    BcbIndicatorValue? SavingsMonthly);
