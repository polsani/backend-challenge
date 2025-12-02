using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Challenge.Infrastructure.Postgres;

public class DefaultDbContext : DbContext
{
    private readonly string _connectionString;
    private readonly IConfiguration _configuration;
    
    public DefaultDbContext(string connectionString)
    {
        _configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();
        
        _connectionString = GetConnectionString();
    }

    private string GetConnectionString()
    {
        var connectionStringTemplate = _configuration.GetConnectionString("DefaultConnection");
        
        var dbUsername = Environment.GetEnvironmentVariable("POSTGRES_USER") 
                         ?? throw new InvalidOperationException("POSTGRES_USER environment variable is not set");
        var dbPassword = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD") 
                         ?? throw new InvalidOperationException("POSTGRES_PASSWORD environment variable is not set");
        
        var connectionString = string.Format(connectionStringTemplate ?? throw new InvalidOperationException(), 
            dbUsername, dbPassword);
        
        return connectionString;
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(_connectionString);
    }
}