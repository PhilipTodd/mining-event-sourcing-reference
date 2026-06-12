using BlastPlanning.Application.Abstractions.Clock;
using BlastPlanning.Application.Abstractions.EventStore;
using BlastPlanning.Application.Abstractions.Messaging;
using BlastPlanning.Application.Abstractions.ReadModels;
using BlastPlanning.Infrastructure.Clock;
using BlastPlanning.Infrastructure.EventStore.Cosmos;
using BlastPlanning.Infrastructure.EventStore.InMemory;
using BlastPlanning.Infrastructure.Messaging;
using BlastPlanning.Infrastructure.Persistence.Sql;
using BlastPlanning.Infrastructure.Projections.BlastPlans;
using BlastPlanning.Infrastructure.Projections.InMemory;
using BlastPlanning.Infrastructure.Projections.Sql;
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

        if (configuration.GetValue<bool>("UseInMemoryReadModels"))
        {
            services.AddSingleton<IBlastPlanReadRepository, InMemoryBlastPlanReadRepository>();
        }
        else
        {
            services.AddScoped<IBlastPlanReadRepository, SqlBlastPlanReadRepository>();
        }

        services.AddScoped<BlastPlanProjector>();

        if (configuration.GetValue<bool>("UseInMemoryEventStore"))
        {
            services.AddScoped<IEventStore, InMemoryEventStore>();
        }
        else
        {
            services.AddSingleton(sp =>
            {
                var options = sp.GetRequiredService<
                    Microsoft.Extensions.Options.IOptions<CosmosEventStoreOptions>>().Value;

                return new CosmosClient(options.ConnectionString);
            });

            services.AddScoped<IEventStore, CosmosEventStore>();
        }

        services.AddScoped<SqlConnectionFactory>();
        services.AddScoped<IEventPublisher, InProcessEventPublisher>();

        return services;
    }
}