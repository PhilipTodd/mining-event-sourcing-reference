using Azure.Messaging.ServiceBus;
using BlastPlanning.Infrastructure;
using BlastPlanning.ProjectionWorker;
using BlastPlanning.ProjectionWorker.Messaging;
using BlastPlanning.ProjectionWorker.Services;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

builder.Services.AddSingleton<EventDeserializer>();
builder.Services.AddScoped<ProjectionProcessor>();
builder.Services.AddInfrastructure(builder.Configuration);
//builder.Services.AddSingleton<ServiceBusClient>(...);

var host = builder.Build();
host.Run();
