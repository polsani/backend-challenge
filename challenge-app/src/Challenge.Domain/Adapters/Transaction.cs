using Challenge.Domain.Entities;
using Challenge.Domain.ValueObjects;

namespace Challenge.Domain.Adapters;

using DataTransferObjects;
using Mapster;

public static class Transaction
{
    public static void Configure()
    {
        TypeAdapterConfig<TransactionIn, Entities.Transaction>
            .NewConfig()
            .MapWith(src => new Entities.Transaction
            {
                Type = TransactionType.Get(ushort.Parse(src.Type)),
                TaxId = new TaxId(src.TaxId),
                Card = src.Card,
                Time = TimeOnly.Parse(
                    $"{src.Time.Substring(0, 2)}:{src.Time.Substring(2, 2)}:{src.Time.Substring(4, 2)}"),
                Date = DateOnly.Parse(
                    $"{src.Date.Substring(0, 4)}-{src.Date.Substring(4, 2)}-{src.Date.Substring(6, 2)}"),
                Value = decimal.Parse(src.Value) / 100,
                StoreOwner = src.StoreOwner.Trim(),
                StoreName = src.StoreName.Trim()
            });
        
        TypeAdapterConfig<Entities.Transaction, TransactionOut>
            .NewConfig()
            .MapWith(src => new TransactionOut
            {
                Id = src.Id,
                Type = src.Type.Description,
                TaxId = src.TaxId.FormattedTaxId,
                Card = src.Card,
                Value = src.Value,
                StoreOwner = src.StoreOwner,
                StoreName = src.StoreName,
                Date = src.Date.ToString("yyyy-MM-dd"),
                Time = src.Time.ToString("HH:mm:ss")
            });
    }
}