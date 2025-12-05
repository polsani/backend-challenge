using System.Text;
using Challenge.Domain.Contracts.Repository;
using Challenge.Domain.Contracts.Services;
using Challenge.Domain.Contracts.Storage;
using Challenge.Domain.Entities;
using Challenge.Domain.ValueObjects;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace Challenge.Domain.Services;

public class ImporterService(IValidator<Transaction> transactionValidator,
    ITransactionRepository repository,
    IStorageService storageService,
    ILogger<ImporterService> logger) : IImporterService
{
    public async Task<ImportResult> ImportFromStreamAsync(Stream stream, string fileName)
    {
        var result = new ImportResult();
        var invalidTransactions = new List<string>();
        var validTransactions = new List<Transaction>();
        var lineNumber = 0;
        
        using var reader = new StreamReader(stream);
        
        string? line;
        while ((line = await reader.ReadLineAsync()) is not null)
        {
            lineNumber++;
            
            if(string.IsNullOrWhiteSpace(line))
                continue;
            
            result.TotalLines++;
            var transaction = Transaction.FromString(line);
            var validationResult = await transactionValidator.ValidateAsync(transaction);

            if (!validationResult.IsValid)
            {
                logger.LogWarning("Invalid transaction:{LineNumber} - {Line}", lineNumber, line);
                validationResult.Errors.ForEach(x=> 
                    logger.LogWarning("Validation error at line {LineNumber}: {message}", lineNumber, x.ErrorMessage));
                invalidTransactions.Add(line);
                result.FailedCount++;
                continue;
            }
            
            validTransactions.Add(transaction);
            result.SuccessCount++;
        }
        
        await repository.ImportTransactionsAsync(validTransactions);
        await ExportInvalidTransactionsAsync(invalidTransactions, fileName);
        
        return result;
    }

    public async Task ExportInvalidTransactionsAsync(IEnumerable<string> invalidTransactions, string fileName)
    {
        var content = string.Join(Environment.NewLine, invalidTransactions);
        var bytes = Encoding.UTF8.GetBytes(content);
        var stream = new MemoryStream(bytes);

        await storageService.UploadFailedFileAsync(stream, fileName, "application/octet-stream");
    }
}