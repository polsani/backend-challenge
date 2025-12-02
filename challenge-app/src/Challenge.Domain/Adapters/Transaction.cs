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
            .Map(dest => dest.Type, src => TransactionType.Get(ushort.Parse(src.Type)))
            .Map(dest => dest.TaxId, src => new TaxId(src.TaxId))
            .Map(dest => dest.Card, src => src.Card)
            .Map(dest => dest.Time, src => 
                TimeOnly.Parse($"{src.Time.Substring(0,2)}:{src.Time.Substring(2,2)}:{src.Time.Substring(4,2)}"))
            .Map(dest => dest.Date, src => 
                DateOnly.Parse($"{src.Date.Substring(0,4)}-{src.Date.Substring(4,2)}-{src.Date.Substring(6,2)}"))
            .Map(dest => dest.Value, src => decimal.Parse(src.Value) / 100)
            .Map(dest => dest.StoreOwner, src => src.StoreOwner)
            .Map(dest => dest.StoreName, src => src.StoreName);
        
        TypeAdapterConfig<Entities.Transaction, TransactionOut>
            .NewConfig()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Type, src => src.Type.Description)
            .Map(dest => dest.TaxId, src => src.TaxId.FormattedTaxId)
            .Map(dest => dest.Card, src => src.Card)
            .Map(dest => dest.Value, src => src.Value)
            .Map(dest => dest.StoreOwner, src => src.StoreOwner)
            .Map(dest => dest.StoreName, src => src.StoreName)
            .Map(dest => dest.Date, src => src.Date.ToString("yyyy-MM-dd"))
            .Map(dest => dest.Time, src => src.Time.ToString("HH:mm:ss"));
    }
}