using BlastPlanning.Application.Abstractions;
using BlastPlanning.Application.Abstractions.ReadModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace BlastPlanning.Api.Tests.Support;

public sealed class BlastPlanningApiFactory
    : WebApplicationFactory<Program>
{
    public FakeBlastPlanReadRepository ReadRepository { get; } = new();

    protected override void ConfigureWebHost(
        IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<IBlastPlanReadRepository>();

            services.AddSingleton<IBlastPlanReadRepository>(
                ReadRepository);
        });
    }
}