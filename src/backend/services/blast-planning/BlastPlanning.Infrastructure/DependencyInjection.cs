using BlastPlanning.Application.Abstractions.Clock;
using BlastPlanning.Application.Abstractions.EventStore;
using BlastPlanning.Infrastructure.Clock;
using BlastPlanning.Infrastructure.EventStore.Cosmos;
using BlastPlanning.Infrastructure.Persistence.Sql;
using BlastPlanning.Infrastructure.EventStore.InMemory;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BlastPlanning.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<CosmosEventStoreOptions>(
            configuration.GetSection(CosmosEventStoreOptions.SectionName));

        services.Configure<SqlOptions>(
            configuration.GetSection(SqlOptions.SectionName));

        services.AddSingleton<IClock, SystemClock>();
        services.AddSingleton<IEventStore, InMemoryEventStore>();

        // commenting the below until Cosmos integration is complete. Temporarily using InMemoryEventStore for dev.
        //services.AddScoped<IEventStore, CosmosEventStore>();
        //services.AddSingleton(sp =>
        //{
        //    var options = sp.GetRequiredService<
        //        Microsoft.Extensions.Options.IOptions<CosmosEventStoreOptions>>().Value;

        //    return new CosmosClient(options.ConnectionString);
        //});

        services.AddScoped<SqlConnectionFactory>();

        return services;
    }
}