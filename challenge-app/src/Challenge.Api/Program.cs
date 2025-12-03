using Challenge.Domain.Adapters;
using Challenge.Domain.DataTransferObjects;
using Challenge.Infrastructure.Minio;
using Challenge.Infrastructure.Postgres;
using FluentValidation;
using Mapster;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddDbContext<DefaultDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddRepositories();
builder.Services.AddStorage();
builder.Services.AddMapster();

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