using Challenge.Domain.Contracts.Repository;
using Challenge.Domain.Entities;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Npgsql;
using NpgsqlTypes;

namespace Challenge.Infrastructure.Postgres.Repository;

public class TransactionRepository(IConfiguration configuration) : ITransactionRepository
{
    public async Task ImportTransactionsAsync(IEnumerable<Transaction> transactions)
    {
        await using var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection"));
        await connection.OpenAsync();
    
        await using var writer = await connection.BeginBinaryImportAsync(
            """
            COPY transactions (type, date, value, cpf, card, time, 
                      store_owner, store_name) 
                      FROM STDIN (FORMAT BINARY)
            """);
    
        foreach (var t in transactions)
        {
            await writer.StartRowAsync();
            await writer.WriteAsync((short)t.Type.Type, NpgsqlDbType.Smallint);
            await writer.WriteAsync(t.Date, NpgsqlDbType.Date);
            await writer.WriteAsync(t.Value, NpgsqlDbType.Numeric);
            await writer.WriteAsync(t.TaxId.Value);
            await writer.WriteAsync(t.Card);
            await writer.WriteAsync(t.Time, NpgsqlDbType.Time);
            await writer.WriteAsync(t.StoreOwner);
            await writer.WriteAsync(t.StoreName);
        }
    
        await writer.CompleteAsync();
    }
}
