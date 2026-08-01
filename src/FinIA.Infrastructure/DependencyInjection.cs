using FinIA.Application.Configuration;
using FinIA.Application.External.Bcb;
using FinIA.Application.External.Brapi;
using FinIA.Application.Persistence;
using FinIA.Infrastructure.External.Bcb;
using FinIA.Infrastructure.External.Brapi;
using FinIA.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FinIA.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        _ = configuration;
        services.AddDbContext<FinIaDbContext>((provider, options) =>
        {
            var finIaOptions = provider.GetRequiredService<FinIaOptions>();
            if (!string.IsNullOrWhiteSpace(finIaOptions.SupabaseConnectionString))
            {
                options.UseNpgsql(finIaOptions.SupabaseConnectionString);
            }
        });

        services.AddScoped<IAnalysisRequestRepository, EfAnalysisRequestRepository>();

        services.AddHttpClient<IBcbClient, BcbClient>((provider, client) =>
        {
            BcbClient.Configure(client, provider.GetRequiredService<FinIaOptions>());
        });

        services.AddHttpClient<IBrapiClient, BrapiClient>((provider, client) =>
        {
            BrapiClient.Configure(client, provider.GetRequiredService<FinIaOptions>());
        });

        return services;
    }
}
