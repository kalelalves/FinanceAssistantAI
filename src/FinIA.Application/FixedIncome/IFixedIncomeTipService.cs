namespace FinIA.Application.FixedIncome;

public interface IFixedIncomeTipService
{
    bool CanHandle(string message);

    FixedIncomeTipResponse BuildTips(string message);
}
