public class TransactionMap : CsvHelper.Configuration.ClassMap<Transaction>
{
    public TransactionMap()
    {
        Map(m => m.Date);
        Map(m => m.Narrative);
        Map(m => m.From);
        Map(m => m.To);
        Map(m => m.Amount);
    }
}
