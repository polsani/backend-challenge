using Challenge.Domain.ValueObjects;

namespace Challenge.Domain.Entities;

public class TransactionType(
    ushort type,
    string description,
    OperationNature nature,
    OperationSign sign)
{
    public ushort Type { get; init; } = type;
    public string Description { get; init; } = description;
    public OperationNature Nature { get; init; } = nature;
    public OperationSign Sign { get; init; } = sign;

    public static TransactionType Get(ushort type) => TransactionTypes.First(x => x.Type == type);

    private static TransactionType[] TransactionTypes =>
    [
        new (1, "Debit", OperationNature.Income, OperationSign.Plus),
        new (2, "Boleto", OperationNature.Expense, OperationSign.Minus),
        new (3, "Financing", OperationNature.Expense, OperationSign.Minus),
        new (4, "Credit", OperationNature.Income, OperationSign.Plus),
        new (5, "Loan Receipt", OperationNature.Income, OperationSign.Plus),
        new (6, "Sales", OperationNature.Income, OperationSign.Plus),
        new (7, "TED Receipt", OperationNature.Income, OperationSign.Plus),
        new (8, "DOC Receipt", OperationNature.Income, OperationSign.Plus),
        new (9, "Rent", OperationNature.Expense, OperationSign.Minus)
    ];
}