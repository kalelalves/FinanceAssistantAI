using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using FinIA.Application.Ai;
using FinIA.Application.Configuration;

namespace FinIA.Infrastructure.Ai;

public sealed class OpenAiAnalysisService(HttpClient httpClient, FinIaOptions options) : IAiAnalysisService
{
    public async Task<AiAnalysisResponse> AnalyzeAsync(AiAnalysisRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(options.OpenAiApiKey))
        {
            return AiFallbackFormatter.Format(request.FundamentalResult);
        }

        var prompt = BuildPrompt(request);
        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "/v1/chat/completions")
        {
            Content = JsonContent.Create(new ChatCompletionRequest(
                Model: options.OpenAiModel,
                Messages:
                [
                    new ChatMessage("system", "Responda em portugues, de forma objetiva, em no maximo 3 frases."),
                    new ChatMessage("user", prompt)
                ],
                MaxTokens: 120,
                Temperature: 0.2m))
        };
        httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", options.OpenAiApiKey);

        try
        {
            var response = await httpClient.SendAsync(httpRequest, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                return AiFallbackFormatter.Format(request.FundamentalResult);
            }

            var completion = await response.Content.ReadFromJsonAsync<ChatCompletionResponse>(cancellationToken);
            var content = completion?.Choices?.FirstOrDefault()?.Message?.Content;

            if (string.IsNullOrWhiteSpace(content))
            {
                return AiFallbackFormatter.Format(request.FundamentalResult);
            }

            var fundamental = request.FundamentalResult;
            return new AiAnalysisResponse(
                fundamental.Ticker,
                fundamental.CurrentPrice,
                fundamental.TargetPrice,
                fundamental.Horizon,
                fundamental.Diagnosis,
                content.Trim(),
                Source: "openai");
        }
        catch (HttpRequestException)
        {
            return AiFallbackFormatter.Format(request.FundamentalResult);
        }
        catch (TaskCanceledException)
        {
            return AiFallbackFormatter.Format(request.FundamentalResult);
        }
    }

    public static void Configure(HttpClient client)
    {
        client.BaseAddress = new Uri("https://api.openai.com");
        client.Timeout = TimeSpan.FromSeconds(20);
    }

    private static string BuildPrompt(AiAnalysisRequest request)
    {
        var result = request.FundamentalResult;
        var reasons = string.Join("; ", result.Reasons);
        return $"""
            Ticker: {result.Ticker}
            Preco atual: {result.CurrentPrice}
            Preco-alvo calculado: {result.TargetPrice}
            Horizonte calculado: {result.Horizon}
            Diagnostico calculado: {result.Diagnosis}
            Score: {result.Score}
            Razoes: {reasons}

            Gere somente uma justificativa objetiva mantendo os numeros calculados pelo backend.
            """;
    }

    private sealed record ChatCompletionRequest(
        [property: JsonPropertyName("model")] string Model,
        [property: JsonPropertyName("messages")] IReadOnlyCollection<ChatMessage> Messages,
        [property: JsonPropertyName("max_tokens")] int MaxTokens,
        [property: JsonPropertyName("temperature")] decimal Temperature);

    private sealed record ChatMessage(
        [property: JsonPropertyName("role")] string Role,
        [property: JsonPropertyName("content")] string Content);

    private sealed record ChatCompletionResponse(
        [property: JsonPropertyName("choices")] IReadOnlyCollection<ChatChoice>? Choices);

    private sealed record ChatChoice(
        [property: JsonPropertyName("message")] ChatMessage? Message);
}
