using FinIA.Application.FixedIncome;

namespace FinIA.Tests.FixedIncome;

public sealed class FixedIncomeTipServiceTests
{
    [Theory]
    [InlineData("Quero dicas de renda fixa")]
    [InlineData("Vale a pena Tesouro Selic?")]
    [InlineData("Compare CDB com LCI")]
    public void CanHandle_ShouldDetectFixedIncomeIntent(string message)
    {
        var service = new FixedIncomeTipService();

        Assert.True(service.CanHandle(message));
    }

    [Fact]
    public void BuildTips_ShouldReturnObjectiveEducationalTips()
    {
        var service = new FixedIncomeTipService();

        var response = service.BuildTips("Me de dicas de LCI e IPCA");

        Assert.Contains("Nao e recomendacao individual", response.Message);
        Assert.Contains(response.Tips, tip => tip.Title == "Reserva");
        Assert.Contains(response.Tips, tip => tip.Title == "IPCA+");
        Assert.Contains(response.Tips, tip => tip.Title == "Isencao");
    }
}
