namespace Challenge.Domain.ValueObjects;

public readonly struct TaxId
{
    public string Value => _taxId;
    
    private readonly string _taxId;

    public TaxId(string taxId)
    {
        if (string.IsNullOrWhiteSpace(taxId))
            throw new ArgumentException("TaxId should not be empty", nameof(taxId));
        
        _taxId = taxId;
    }
    
    public string FormattedTaxId => $"{_taxId[..3]}.{_taxId.Substring(3, 3)}.{_taxId.Substring(6, 3)}-{_taxId.Substring(9, 2)}";
    public override string ToString() => FormattedTaxId;
    
    private static bool ValidateTaxId(string cpf)
    {
        

        if (new string(cpf[0], 11) == cpf)
            return false;

        // Validação dos dígitos verificadores
        int[] multiplicador1 = { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
        int[] multiplicador2 = { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

        string tempCpf = cpf.Substring(0, 9);
        int soma = 0;

        for (int i = 0; i < 9; i++)
            soma += int.Parse(tempCpf[i].ToString()) * multiplicador1[i];

        int resto = soma % 11;
        resto = resto < 2 ? 0 : 11 - resto;

        string digito = resto.ToString();
        tempCpf += digito;
        soma = 0;

        for (int i = 0; i < 10; i++)
            soma += int.Parse(tempCpf[i].ToString()) * multiplicador2[i];

        resto = soma % 11;
        resto = resto < 2 ? 0 : 11 - resto;
        digito += resto.ToString();

        return cpf.EndsWith(digito);
    }
}