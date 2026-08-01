using System.Text.Json;
using FinIA.Application.Analyses;
using FinIA.Application.Auth;
using FinIA.Application.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;

namespace FinIA.Functions.Functions;

public sealed class CreateAnalysisFunction(
    IBearerTokenReader bearerTokenReader,
    ISupabaseJwtValidator jwtValidator,
    IAnalysisRequestValidator requestValidator,
    IAnalysisApplicationService analysisApplicationService)
{
    [Function("CreateAnalysis")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "analyses")] HttpRequest request,
        CancellationToken cancellationToken)
    {
        var token = bearerTokenReader.Read(request.Headers.Authorization);
        var authResult = jwtValidator.Validate(token);

        if (!authResult.IsAuthenticated || authResult.User is null)
        {
            return new UnauthorizedObjectResult(new ApiError(
                authResult.ErrorCode ?? "auth.unauthorized",
                authResult.ErrorMessage ?? "Unauthorized."));
        }

        var payload = await JsonSerializer.DeserializeAsync<CreateAnalysisRequest>(
            request.Body,
            new JsonSerializerOptions(JsonSerializerDefaults.Web),
            cancellationToken);

        var validation = requestValidator.Validate(payload);
        if (!validation.IsValid)
        {
            return new BadRequestObjectResult(new ApiError(
                validation.ErrorCode ?? "analysis.invalid_request",
                validation.ErrorMessage ?? "Invalid analysis request."));
        }

        var response = await analysisApplicationService.CreateAsync(
            authResult.User,
            validation.NormalizedTickers,
            cancellationToken);

        return new AcceptedResult($"/api/analyses/{response.AnalysisId}", response);
    }
}
