using Challenge.Api.ErrorHandling;
using Challenge.Domain;
using Challenge.Domain.Adapters;
using Challenge.Infrastructure.Minio;
using Challenge.Infrastructure.Postgres;
using FluentValidation;
using Mapster;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Metrics;
using Serilog;
using Transaction = Challenge.Domain.Entities.Transaction;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.File("logs/app-.log", rollingInterval: RollingInterval.Day, 
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
    .CreateLogger();

try
{
    Log.Information("Starting web application");

    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddControllers();
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAll", policy =>
        {
            policy.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
    });

    builder.Services.AddOpenTelemetry()
        .WithMetrics(metrics =>
        {
            metrics.AddPrometheusExporter()
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation();
        });
    
    builder.Services
        .AddOpenApi()
        .AddEndpointsApiExplorer()
        .AddDbContext<DefaultDbContext>(options =>
        {
            options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
        })
        .AddValidatorsFromAssemblyContaining<Transaction>()
        .AddRepositories()
        .AddDomain()
        .AddStorage(builder.Configuration)
        .AddMapster() ;

    AdapterModule.ConfigureAll();
    builder.Host.UseSerilog();

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
    }
    else
    {
        app.UseHttpsRedirection();
    }

    app.MapPrometheusScrapingEndpoint();
    
    app.UseSerilogRequestLogging();
    app.UseMiddleware<ErrorHandlingMiddleware>();
    app.UseAuthorization();
    app.UseCors("AllowAll");
    app.MapControllers();
    app.Run();
}
catch (Exception e)
{
    Log.Fatal(e, "Application terminated unexpectedly");
}