using Challenge.Domain.Adapters;
using Challenge.Domain.DataTransferObjects;
using Challenge.Infrastructure.Minio;
using Challenge.Infrastructure.Postgres;
using FluentValidation;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Serilog;

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
    builder.Services
        .AddOpenApi()
        .AddDbContext<DefaultDbContext>(options =>
        {
            options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
        })
        .AddValidatorsFromAssemblyContaining<Program>()
        .AddRepositories()
        .AddStorage(builder.Configuration)
        .AddMapster();

    Transaction.Configure();

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
        await using (var serviceScope = app.Services.CreateAsyncScope()) ;
    
    }

    app.UseHttpsRedirection();
    app.UseAuthorization();
    app.MapControllers();
    app.Run();
}
catch (Exception e)
{
    Log.Fatal(e, "Application terminated unexpectedly");
    throw;
}