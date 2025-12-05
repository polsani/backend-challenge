namespace Challenge.Domain.Adapters;

public static class AdapterModule
{
    public static void ConfigureAll()
    {
        Transaction.Configure();
        ImportResult.Configure();
    }
}