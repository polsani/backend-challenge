namespace Challenge.Domain.ValueObjects;

public readonly struct TaxId(string taxId)
{
    public string Value => taxId;

    public string FormattedTaxId => $"{taxId[..3]}.{taxId.Substring(3, 3)}.{taxId.Substring(6, 3)}-{taxId.Substring(9, 2)}";
    public override string ToString() => FormattedTaxId;
}