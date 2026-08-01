using FinIA.Application.Analyses;
using FinIA.Application.Auth;
using FinIA.Application.Configuration;
using FinIA.Application.Fundamentals;
using FinIA.Application.Health;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FinIA.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        var options = new FinIaOptions
        {
            BcbBaseUrl = configuration["BCB_BASE_URL"] ?? "https://api.bcb.gov.br",
            BrapiBaseUrl = configuration["BRAPI_BASE_URL"] ?? "https://brapi.dev",
            BrapiToken = configuration["BRAPI_TOKEN"],
            OpenAiApiKey = configuration["OPENAI_API_KEY"],
            OpenAiModel = configuration["OPENAI_MODEL"] ?? "gpt-4o-mini",
            SupabaseUrl = configuration["SUPABASE_URL"],
            SupabaseJwtSecret = configuration["SUPABASE_JWT_SECRET"],
            SupabaseConnectionString = configuration["SUPABASE_CONNECTION_STRING"],
            MaxAssetsPerAnalysis = int.TryParse(configuration["MAX_ASSETS_PER_ANALYSIS"], out var maxAssets)
                ? maxAssets
                : FinIA.Domain.Analysis.AnalysisLimits.MaxAssetsPerRequest
        };

        services.AddSingleton(options);
        services.AddSingleton(FinIaOptionsValidator.Validate(options));
        services.AddSingleton<IBearerTokenReader, BearerTokenReader>();
        services.AddSingleton<ISupabaseJwtValidator, SupabaseJwtValidator>();
        services.AddSingleton<IHealthService, HealthService>();
        services.AddSingleton<IAnalysisRequestValidator, AnalysisRequestValidator>();
        services.AddSingleton<IAnalysisApplicationService, AnalysisApplicationService>();
        services.AddSingleton<IFundamentalAnalysisService, FundamentalAnalysisService>();

        return services;
    }
}
