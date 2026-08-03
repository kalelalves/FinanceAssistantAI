namespace FinIA.Application.FixedIncome;

public sealed record FixedIncomeTipResponse(
    string Message,
    IReadOnlyCollection<FixedIncomeTip> Tips);
