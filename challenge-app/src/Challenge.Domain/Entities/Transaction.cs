using System.ComponentModel.DataAnnotations;
using Challenge.Domain.DataTransferObjects;
using Challenge.Domain.ValueObjects;
using Mapster;

namespace Challenge.Domain.Entities;

public class Transaction
{
    [Key]
    public int Id { get; set; }
    public required TransactionType Type { get; init; }
    public required DateOnly Date { get; init; }
    public required decimal Value { get; init; }
    public required TaxId TaxId { get; init; }
    public required string Card { get; init; }
    public required TimeOnly Time { get; init; }
    public required string StoreOwner { get; init; }
    public required string StoreName { get; init; }

    /// <summary>
    /// This method creates a new instance of Transaction using a specific string format <br/>
    /// | Field       | Start | End | Size |<br/>
    /// | Type	      |    1  |  1  |	1  |<br/>
    /// | Date	      |    2  |  9  |   8  |<br/>
    /// | Value       |   10  | 19  |  10  |<br/>
    /// | TaxId       |   20  | 30  |  11  |<br/>
    /// | Card	      |   31  |	42	|  12  |<br/>
    /// | Time        |   43  | 48	|  6   |<br/>
    /// | Store Owner |   49  | 62	|  14  |<br/>
    /// | Store Name  |	  63  | 81	|  19  |
    /// </summary>
    /// <param name="rawTransaction"></param>
    /// <returns></returns>
    public static Transaction FromString(string rawTransaction)
    {
        var transactionIn = new TransactionIn
        {
            Type = rawTransaction[..1],
            Date = rawTransaction[1..9],
            Value = rawTransaction[9..19],
            TaxId = rawTransaction[19..30],
            Card =  rawTransaction[30..42],
            Time =  rawTransaction[42..48],
            StoreOwner = rawTransaction[48..62],
            StoreName = rawTransaction[62..]
        };
        
        return transactionIn.Adapt<Transaction>();
    }
}