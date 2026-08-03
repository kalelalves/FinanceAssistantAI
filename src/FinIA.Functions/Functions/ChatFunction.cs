using System.Text.Json;
using System.Text.RegularExpressions;
using FinIA.Application.Analyses;
using FinIA.Application.Auth;
using FinIA.Application.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;

namespace FinIA.Functions.Functions;

public sealed partial class ChatFunction(
    IAnalysisRequestValidator requestValidator,
    IAnalysisApplicationService analysisApplicationService)
{
    private static readonly AuthenticatedUser ChatUser = new(
        Guid.Parse("11111111-1111-1111-1111-111111111111"),
        "chat-local@finia.local");

    [Function("Chat")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "chat")] HttpRequest request,
        CancellationToken cancellationToken)
    {
        var payload = await JsonSerializer.DeserializeAsync<ChatRequest>(
            request.Body,
            new JsonSerializerOptions(JsonSerializerDefaults.Web),
            cancellationToken);

        if (string.IsNullOrWhiteSpace(payload?.Message))
        {
            return new BadRequestObjectResult(new ApiError("chat.empty_message", "Message is required."));
        }

        var tickers = TickerRegex()
            .Matches(payload.Message.ToUpperInvariant())
            .Select(match => match.Value)
            .Distinct()
            .ToArray();

        var validation = requestValidator.Validate(new CreateAnalysisRequest(tickers));
        if (!validation.IsValid)
        {
            return new BadRequestObjectResult(new ApiError(
                validation.ErrorCode ?? "analysis.invalid_request",
                validation.ErrorMessage ?? "Invalid analysis request."));
        }

        var analysis = await analysisApplicationService.CreateAsync(
            ChatUser,
            validation.NormalizedTickers,
            cancellationToken);

        return new OkObjectResult(new ChatResponse(
            Message: BuildMessage(analysis.Results),
            Results: analysis.Results.Select(ChatAssetResponse.From).ToArray()));
    }

    private static string BuildMessage(IReadOnlyCollection<AssetAnalysisResponse> results)
    {
        return results.Count == 0
            ? "Nao encontrei resultados para os papeis informados."
            : $"Analise concluida para {results.Count} papel(is).";
    }

    [GeneratedRegex(@"\b[A-Z]{4}\d{1,2}\b")]
    private static partial Regex TickerRegex();

    private sealed record ChatRequest(string? Message);

    private sealed record ChatResponse(
        string Message,
        IReadOnlyCollection<ChatAssetResponse> Results);

    private sealed record ChatAssetResponse(
        string Ticker,
        decimal? CurrentPrice,
        decimal? TargetPrice,
        string Horizon,
        string Diagnosis,
        string Summary,
        string Source)
    {
        public static ChatAssetResponse From(AssetAnalysisResponse response)
        {
            return new ChatAssetResponse(
                response.Ticker,
                response.CurrentPrice,
                response.TargetPrice,
                response.Horizon.ToString(),
                response.Diagnosis.ToString(),
                response.Summary,
                response.Source);
        }
    }
}
