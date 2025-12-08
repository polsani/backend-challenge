using System.Diagnostics;
using System.Text;
using Challenge.Domain.Contracts.Repository;
using Challenge.Domain.Contracts.Services;
using Challenge.Domain.DataTransferObjects;
using Challenge.Domain.Entities;
using Challenge.Domain.ValueObjects;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace Challenge.Domain.Services;

public class ImporterService(
    IValidator<Transaction> transactionValidator,
    ITransactionRepository repository,
    ILogger<ImporterService> logger) : IImporterService
{
    private const int BatchSize = 10000;
    private const int StreamReaderBufferSize = 65536;
    
    public async Task<ImportResult> ImportFromStreamAsync(Stream stream, string fileName, bool showSummary = false)
    {
        var result = new ImportResult();
        var stopwatch = Stopwatch.StartNew();
        
        if (showSummary)
            result.Summary = new Dictionary<string, SummaryResult>();
        
        try
        {
            await ProcessStreamAsync(stream, result, showSummary, stopwatch);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error importing file {FileName} at line {LineNumber}", fileName, result.TotalLines);
            throw;
        }
        finally
        {
            stopwatch.Stop();
            logger.LogInformation(
                "Import completed for {FileName}: {SuccessCount} successful, {FailedCount} failed, " +
                "Total: {TotalLines} in {ElapsedTime}ms",
                fileName, result.SuccessCount, result.FailedCount, result.TotalLines, stopwatch.ElapsedMilliseconds);
        }
        
        return result;
    }
    
    private async Task ProcessStreamAsync(Stream stream, ImportResult result, bool showSummary, Stopwatch stopwatch)
    {
        using var reader = new StreamReader(stream, Encoding.UTF8, bufferSize: StreamReaderBufferSize, leaveOpen: true);
        
        var batch = new List<Transaction>(BatchSize);
        int lineNumber = 0;
        long totalProcessed = 0;
        
        string? line;
        while ((line = await reader.ReadLineAsync()) is not null)
        {
            lineNumber++;
            
            if (string.IsNullOrWhiteSpace(line))
                continue;
            
            var parseResult = TryParseTransaction(line, lineNumber);
            if (!parseResult.IsSuccess)
            {
                result.FailedCount++;
                totalProcessed++;
                continue;
            }
            
            var transaction = parseResult.Transaction!;
            var isValid = await ValidateTransactionAsync(transaction, line, lineNumber);
            
            if (!isValid)
            {
                result.FailedCount++;
                totalProcessed++;
                continue;
            }
            
            if (showSummary)
                UpdateSummary(result.Summary!, transaction);
            
            batch.Add(transaction);
            result.SuccessCount++;
            
            if (batch.Count >= BatchSize)
            {
                await FlushBatchAsync(batch, repository);
                totalProcessed += batch.Count;
                batch.Clear();
                
                LogProgress(totalProcessed, stopwatch);
            }
        }
        
        if (batch.Count > 0)
        {
            await FlushBatchAsync(batch, repository);
            totalProcessed += batch.Count;
        }
        
        result.TotalLines = totalProcessed;
    }
    
    private (bool IsSuccess, Transaction? Transaction) TryParseTransaction(string line, int lineNumber)
    {
        try
        {
            var transaction = Transaction.FromString(line);
            return (true, transaction);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to parse transaction at line {LineNumber}: {Line}", lineNumber, line);
            return (false, null);
        }
    }
    
    private async Task<bool> ValidateTransactionAsync(Transaction transaction, string line, int lineNumber)
    {
        var validationResult = await transactionValidator.ValidateAsync(transaction);
        
        if (validationResult.IsValid)
            return true;
        
        logger.LogWarning("Invalid transaction at line {LineNumber}: {Line}", lineNumber, line);        
        validationResult.Errors.ForEach(error => logger.LogWarning("Validation error at line {LineNumber}: {ErrorMessage}", lineNumber, error.ErrorMessage));
        
        return false;
    }
    
    private static void UpdateSummary(Dictionary<string, SummaryResult> summary, Transaction transaction)
    {
        if (!summary.TryGetValue(transaction.StoreName, out var summaryResult))
        {
            summaryResult = new SummaryResult();
            summary[transaction.StoreName] = summaryResult;
        }
        
        summaryResult.Transactions.Add(transaction);
        
        var balanceAdjustment = transaction.Type.Sign == OperationSign.Plus 
            ? transaction.Value 
            : -transaction.Value;
        
        summaryResult.Balance += balanceAdjustment;
    }
    
    private static async Task FlushBatchAsync(List<Transaction> batch, ITransactionRepository repository)
    {
        if (batch.Count == 0)
            return;
        
        await repository.ImportTransactionsAsync(batch);
    }
    
    private void LogProgress(long totalProcessed, Stopwatch stopwatch)
    {
        if (stopwatch.Elapsed.TotalSeconds <= 0)
            return;
        
        var speed = totalProcessed / stopwatch.Elapsed.TotalSeconds;
        logger.LogDebug(
            "Processed: {Total:N0} | Speed: {Speed:N0} rec/s | Elapsed time: {ElapsedTime}",
            totalProcessed, speed, stopwatch.Elapsed);
    }
}
