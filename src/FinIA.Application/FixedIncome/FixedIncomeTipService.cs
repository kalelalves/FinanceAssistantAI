using System.Globalization;
using System.Text;

namespace FinIA.Application.FixedIncome;

public sealed class FixedIncomeTipService : IFixedIncomeTipService
{
    private static readonly string[] IntentTerms =
    [
        "RENDA FIXA",
        "TESOURO",
        "CDB",
        "LCI",
        "LCA",
        "CRI",
        "CRA",
        "DEBENTURE",
        "DEBENTURES",
        "CDI",
        "SELIC",
        "IPCA",
        "POUPANCA"
    ];

    public bool CanHandle(string message)
    {
        var normalized = Normalize(message);
        return IntentTerms.Any(normalized.Contains);
    }

    public FixedIncomeTipResponse BuildTips(string message)
    {
        var normalized = Normalize(message);
        var tips = new List<FixedIncomeTip>
        {
            new("Reserva", "Para dinheiro de emergencia, priorize liquidez diaria, baixo risco e pos-fixado ao CDI/Selic."),
            new("Prazo", "Evite travar vencimentos longos se existe chance de precisar do dinheiro antes."),
            new("Risco", "CDB, LCI e LCA contam com FGC dentro dos limites vigentes; Tesouro depende do governo federal."),
            new("Comparacao", "Compare rentabilidade liquida: IR, prazo, liquidez e percentual do CDI mudam o resultado final.")
        };

        if (normalized.Contains("IPCA"))
        {
            tips.Add(new("IPCA+", "Use para objetivos de medio/longo prazo quando quiser proteger poder de compra."));
        }

        if (normalized.Contains("SELIC") || normalized.Contains("TESOURO"))
        {
            tips.Add(new("Tesouro Selic", "Costuma fazer sentido para reserva ou caixa conservador, observando taxas e prazo de resgate."));
        }

        if (normalized.Contains("LCI") || normalized.Contains("LCA"))
        {
            tips.Add(new("Isencao", "LCI/LCA podem ser isentas de IR para pessoa fisica, mas compare liquidez e taxa equivalente ao CDB."));
        }

        return new FixedIncomeTipResponse(
            "Dicas gerais de renda fixa. Nao e recomendacao individual; ajuste ao prazo, risco e necessidade de liquidez.",
            tips);
    }

    private static string Normalize(string value)
    {
        var formD = value.Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder(formD.Length);

        foreach (var character in formD)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(character) != UnicodeCategory.NonSpacingMark)
            {
                builder.Append(char.ToUpperInvariant(character));
            }
        }

        return builder.ToString().Normalize(NormalizationForm.FormC);
    }
}
