using Challenge.Domain.Entities;
using Challenge.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Challenge.Infrastructure.Postgres;

public class DefaultDbContext(DbContextOptions<DefaultDbContext> options) : DbContext(options)
{
    public DbSet<Transaction> Transactions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ConfigureTransaction(modelBuilder);
        ConfigureTransactionType(modelBuilder);
        Seed(modelBuilder);
    }

    private static void ConfigureTransaction(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Transaction>()
            .HasKey(x => x.Id);
        
        modelBuilder.Entity<Transaction>()
            .HasOne(x=>x.Type)
            .WithMany(t=> t.Transactions)
            .HasForeignKey(x=>x.Type);
        
        modelBuilder.Entity<Transaction>()
            .HasIndex(x => x.StoreName)
            .HasDatabaseName("IX_Transaction_StoreName");
    }
    
    private static void ConfigureTransactionType(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TransactionType>()
            .HasIndex(x => x.Type);
    }

    private static void Seed(ModelBuilder modelBuilder)
    {
        var transactionTypes = new List<TransactionType>
        {
            new(1, "Debit", OperationNature.Income, OperationSign.Plus),
            new(2, "Boleto", OperationNature.Expense, OperationSign.Minus),
            new(3, "Financing", OperationNature.Expense, OperationSign.Minus),
            new(4, "Credit", OperationNature.Income, OperationSign.Plus),
            new(5, "Loan Receipt", OperationNature.Income, OperationSign.Plus),
            new(6, "Sales", OperationNature.Income, OperationSign.Plus),
            new(7, "TED Receipt", OperationNature.Income, OperationSign.Plus),
            new(8, "DOC Receipt", OperationNature.Income, OperationSign.Plus),
        };
        
        modelBuilder.Entity<TransactionType>().HasData(transactionTypes);
    }
}