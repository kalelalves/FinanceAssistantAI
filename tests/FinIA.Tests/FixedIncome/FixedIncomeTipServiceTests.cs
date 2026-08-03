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

    [Fact]
    public void BuildTips_ShouldAdaptToAmountAndHorizon()
    {
        var service = new FixedIncomeTipService();

        var response = service.BuildTips("Tenho 5000 reais para curto prazo em Tesouro Selic");

        Assert.Contains(response.Tips, tip => tip.Title == "Valor informado" && tip.Detail.Contains("R$ 5.000"));
        Assert.Contains(response.Tips, tip => tip.Title == "Curto prazo");
        Assert.Contains(response.Tips, tip => tip.Title == "Tesouro Selic");
    }
}
