namespace Challenge.Domain.ValueObjects;

public struct TransactionType(ushort type, string description, OperationNature nature, OperationSign sign)
{
    public ushort Type { get; private set; } = type;
    public string Description { get; private set; } = description;
    public OperationNature Nature { get; private set; } = nature;
    public OperationSign Sign { get; private set; } = sign;
    
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