using BlastPlanning.Application.BlastPlans.Commands.ApproveBlastPlan;
using BlastPlanning.Application.BlastPlans.Commands.CreateBlastPlan;
using BlastPlanning.Infrastructure;
using MediatR;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(CreateBlastPlanCommandHandler).Assembly);
});

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Add a blast plan
app.MapPost("/blast-plans", async (
    CreateBlastPlanRequest request,
    ISender sender,
    CancellationToken cancellationToken) =>
{
    var result = await sender.Send(
        new CreateBlastPlanCommand(
            request.Name,
            request.SiteId),
        cancellationToken);

    return Results.Created(
        $"/blast-plans/{result.BlastPlanId}",
        result);
});

// Approve a blast plan
app.MapPost("/blast-plans/{id:guid}/approve", async (
    Guid id,
    ApproveBlastPlanRequest request,
    ISender sender,
    CancellationToken cancellationToken) =>
{
    await sender.Send(
        new ApproveBlastPlanCommand(id, request.ApprovedBy),
        cancellationToken);

    return Results.NoContent();
});

// Legacy endpoint for testing only - to be removed
var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.Run();

public sealed record CreateBlastPlanRequest(
    string Name,
    string SiteId);

public sealed record ApproveBlastPlanRequest(
    string ApprovedBy);

record WeatherForecast(
    DateOnly Date, 
    int TemperatureC, 
    string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
