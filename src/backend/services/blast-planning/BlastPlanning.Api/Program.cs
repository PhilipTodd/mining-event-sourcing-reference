using BlastPlanning.Application.BlastPlans.Commands.ApproveBlastPlan;
using BlastPlanning.Application.BlastPlans.Commands.CreateBlastPlan;
using BlastPlanning.Application.BlastPlans.Queries.GetBlastPlanSummary;
using BlastPlanning.Application.Common.Exceptions;
using BlastPlanning.Domain.Exceptions;
using BlastPlanning.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Identity.Web;

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

// Add observability services
builder.Services.AddApplicationInsightsTelemetry();
builder.Services.AddHealthChecks();

// Add authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));
builder.Services.AddAuthorization();

// Set Cors according to value in appSettings file
var allowedOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>() ?? [];

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseHttpsRedirection();

app.UseCors("AngularDev");

app.UseAuthentication();
app.UseAuthorization();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapHealthChecks("/health");

// Exception handling at top level to ensure meaningful response to clients
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exception = context.Features
            .Get<IExceptionHandlerFeature>()?
            .Error;

        context.Response.ContentType = "application/problem+json";

        var problem = exception switch
        {
            NotFoundException => new
            {
                title = "Resource not found",
                status = StatusCodes.Status404NotFound,
                detail = exception.Message
            },

            ConcurrencyException => new
            {
                title = "Concurrency conflict",
                status = StatusCodes.Status409Conflict,
                detail = exception.Message
            },

            InvalidBlastPlanStateException => new
            {
                title = "Invalid blast plan state",
                status = StatusCodes.Status409Conflict,
                detail = exception.Message
            },

            DomainValidationException => new
            {
                title = "Domain validation failed",
                status = StatusCodes.Status400BadRequest,
                detail = exception.Message
            },

            _ => new
            {
                title = "Unexpected error",
                status = StatusCodes.Status500InternalServerError,
                detail = "An unexpected error occurred."
            }
        };

        context.Response.StatusCode = problem.status;

        await context.Response.WriteAsJsonAsync(problem);
    });
});



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
}).RequireAuthorization();

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
}).RequireAuthorization();

// Get a blast plan summary
app.MapGet("/blast-plans/{id:guid}", async (
    Guid id,
    ISender sender,
    CancellationToken cancellationToken) =>
{
    var result = await sender.Send(
        new GetBlastPlanSummaryQuery(id),
        cancellationToken);

    return result is null
        ? Results.NotFound()
        : Results.Ok(result);
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
